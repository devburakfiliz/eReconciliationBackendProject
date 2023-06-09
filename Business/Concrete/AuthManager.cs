﻿using Business.Abstract;
using Business.Constans;
using Business.ValidationRule.FluentValidation;
using Core.Aspects.Autofac.Transaction;
using Core.CrossCuttingConcerns.Validation;
using Core.Entities.Concrete;
using Core.Utilities.Hashing;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using Core.Utilities.Security.JWT;
using Entities.Concrete;
using Entities.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenHelper _tokenHelper;
        private readonly ICompanyService _companyService;
        private readonly IMailParameterService _mailParameterService;
        private readonly IMailService _mailService;
        private readonly IMailTemplateService _mailTemplateService;

        public AuthManager(IUserService userService, ITokenHelper tokenHelper,ICompanyService companyService,IMailParameterService mailParameterService,IMailService mailService,IMailTemplateService mailTemplateService)
        {
            _userService = userService;
            _tokenHelper = tokenHelper; 
            _companyService = companyService;
            _mailParameterService = mailParameterService;
            _mailService = mailService;
            _mailTemplateService = mailTemplateService;

        }

        public IDataResult<AccessToken> CreateAccessToken(User user, int companyId)
        {
            var claims = _userService.GetClaims(user,companyId);
            var accessToken = _tokenHelper.CreateToken(user, claims,companyId);
            return new SuccessDataResult<AccessToken>(accessToken, "Giriş Başarılı");
        }

        public IDataResult<User> Login(UserForLogin userForLogin)
        {
            var userToCheck = _userService.GetByMail(userForLogin.Email);
            if (userToCheck == null)
            {
                return new ErrorDataResult<User>(Messages.UserNatFound);
            }
            if (!HashingHelper.VerifyPasswordHash(userForLogin.Password, userToCheck.PasswordHash, userToCheck.PasswordSalt))
            {
                return new ErrorDataResult<User>("Parola hatası");
            }

            return new SuccessDataResult<User>(userToCheck, "Başarılı giriş");

        }


      


        public IDataResult<User> RegisterSecondAccount(UserForRegister userForRegister, string password , int companyId)
        {
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);
            var user = new User
            {
                Email = userForRegister.Email,
                AddedAt = DateTime.Now,
                IsActive = true,
                MailConfirm = false,
                MailConfirmDate = DateTime.Now,
                MailConfirmValue = Guid.NewGuid().ToString(),
                Name = userForRegister.Name,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,


            };
            _userService.Add(user);
            _companyService.UserCompanyAdd(user.Id,companyId);
            SendConfirmEmail(user);
            return new SuccessDataResult<User>(user, "Kayıt oldu");
        }

        [TransactionScopeAspect]
        public IDataResult<UserCompanyDto> Register(UserForRegister userForRegister, string password,Company company)
        {
           

            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);
            var user = new User
            {
                Email = userForRegister.Email,
                AddedAt= DateTime.Now,
                IsActive=true,
                MailConfirm=false,
                MailConfirmDate= DateTime.Now,
                MailConfirmValue=Guid.NewGuid().ToString(),
                Name = userForRegister.Name,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
 

            };

            //ValidationTool.Validate(new UserValidator(), user);
            //ValidationTool.Validate(new CompanyValidator(), company);

            _companyService.Add(company);


            _companyService.UserCompanyAdd(user.Id, company.Id);

            UserCompanyDto userCompanyDto = new UserCompanyDto()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                AddedAt = user.AddedAt,
                CompanyId = company.Id,
                IsActive = true,
                MailConfirm = user.MailConfirm,
                MailConfirmDate = user.MailConfirmDate,
                MailConfirmValue = Guid.NewGuid().ToString(),
                PasswordHash = user.PasswordHash,
                PasswordSalt = user.PasswordSalt,

            };
            SendConfirmEmail(user);


            return new SuccessDataResult<UserCompanyDto>(userCompanyDto, "Kayıt oldu");
        }

        void SendConfirmEmail(User user)
        {
            string subject = "Kullanıcı Kayıt Onay Maili";

            string body = "Kullanıcınız sisteme kayıt oldu. Kaydınızı tamamlamak için aşağıdaki linke tıklamanız gerekmektedir.";

            string link = "https://localhost:7043/api/Auth/confirmuser?value=" + user.MailConfirmValue;
            string linkDescription = "Kayıt olmak için tıklayın.";


            var mailTemplate = _mailTemplateService.GetByTemplateName("Kayıt", 3);
            string templateBody = mailTemplate.Data.Value;
            templateBody = templateBody.Replace("{{title}}", subject);
            templateBody = templateBody.Replace("{{message}}", body);
            templateBody = templateBody.Replace("{{link}}", link);
            templateBody = templateBody.Replace("{{linkDescription}}", linkDescription);


            var mailParameter = _mailParameterService.Get(3);
            SendMailDto sendMailDto = new SendMailDto()
            {
                mailParameter = mailParameter.Data,
                email = user.Email,
                subject = "Kullanıcı Kayıt Onay Maili",
                body = templateBody
            };

            _mailService.SendMail(sendMailDto);
            user.MailConfirmDate = DateTime.Now;
            _userService.Update(user);

        }

        public IResult UserExists(string email)
        {
            if (_userService.GetByMail(email) != null)
            {
                return new ErrorResult("Kullanıcı mevcut");
            }
            return new SuccessResult();
        }

        public IResult CompanyExists(Company company)
        {
            var result = _companyService.CompanyExist(company);

            if (result.Success ==false)
            {
                return new ErrorResult("Bu şirket daha önce kayıt edilmiş.");
            }
            return new SuccessResult();
        }

        public IDataResult<User> GetByMailConfirmValue(string value)
        {
            return new SuccessDataResult<User>(_userService.GetByMailComfirmValue(value));
        }

        public IResult Update(User user)
        {
            _userService.Update(user);
            return new SuccessResult("Mail doğrulama başarıyla tamamlandı.");
        }

        public IDataResult<User> GetById(int id)
        {
            return new SuccessDataResult<User>(_userService.GetById(id));

        }

        IResult IAuthService.SendConfirmEmail(User user)
        {
            if (user.MailConfirm ==true)
            {
                return new ErrorResult("Mail Onayı daha önce yapılmış tekrar onay maili alamazsınız.");
            }

            DateTime confirimMailDate = user.MailConfirmDate;
            DateTime now = DateTime.Now;

            if (confirimMailDate.AddMinutes(5)< now)
            {
                SendConfirmEmail(user);
                return new SuccessResult("Onay Maili Tekrar Gönderildi.");
            }
            else
            {
                return new ErrorResult("Mail onayını 5 dakikada bir gönderebilirsiniz.");
            }

            //if (confirimMailDate.ToShortDateString() == now.ToShortDateString()) 
            //{
            //    if (confirimMailDate.Hour == now.Hour && confirimMailDate.AddMinutes(5).Minute <= now.Minute)
            //    {
            //        SendConfirmEmail(user);
            //        return new SuccessResult("Onay Maili Tekrar Gönderildi.");
            //    }
            //    else
            //    {
            //        return new ErrorResult("Mail onayını 5 dakikada bir gönderebilirsiniz.");
            //    }
            //}
            //SendConfirmEmail(user);
            //return new SuccessResult("Onay Maili Tekrar Gönderildi.");


        }

        public IDataResult<UserCompany> GetCompany(int userId)
        {
            return new SuccessDataResult<UserCompany>(_companyService.GetCompany(userId).Data);
        }
    }
    
}
