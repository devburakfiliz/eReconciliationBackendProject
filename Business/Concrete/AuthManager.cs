using Business.Abstract;
using Business.Constans;
using Core.Entities.Concrete;
using Core.Utilities.Hashing;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using Core.Utilities.Security.JWT;
using Entities.Concrete;
using Entities.Dtos;
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

        public AuthManager(IUserService userService, ITokenHelper tokenHelper,ICompanyService companyService,IMailParameterService mailParameterService,IMailService mailService)
        {
            _userService = userService;
            _tokenHelper = tokenHelper; 
            _companyService = companyService;
            _mailParameterService = mailParameterService;
            _mailService = mailService;

        }

        public IDataResult<AccessToken> CreateAccessToken(User user, int companyId)
        {
            var claims = _userService.GetClaims(user,companyId);
            var accessToken = _tokenHelper.CreateToken(user, claims,companyId);
            return new SuccessDataResult<AccessToken>(accessToken, "Token olusturuldu");
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


        public IDataResult<User> RegisterSecondAccount(UserForRegister userForRegister, string password)
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
            return new SuccessDataResult<User>(user, "Kayıt oldu");
        }


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
            _userService.Add(user);
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

            var mailParameter = _mailParameterService.Get(3);
            SendMailDto sendMailDto = new SendMailDto()
            {
                mailParameter = mailParameter.Data,
                email = user.Email,
                subject = "Kullanıcı Onay maili",
                body = "Kullanıcınız sisteme kayıt oldu. Kaydınızı tamamlamak için aşağıdaki linke tıklamanız gerekmektedir."
            };

            _mailService.SendMail(sendMailDto);

            return new SuccessDataResult<UserCompanyDto>(userCompanyDto, "Kayıt oldu");
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
    }
    
}
