using Business.Abstract;
using Business.ValidationRule.FluentValidation;
using Core.Aspects.Autofac.Validation;
using Core.Entities.Concrete;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class UserManager : IUserService
    {
        private readonly IUserDal _userDal;

        public UserManager(IUserDal userDal)
        {
            _userDal = userDal;
        }
        [ValidationAspect(typeof(UserValidator))]
        public void Add(User user)
        {

            _userDal.Add(user);
        }

        public User GetById(int id)
        {
            return _userDal.Get(u=>u.Id == id);
        }

        public User GetByMail(string mail)
        {
            return _userDal.Get(p=>p.Email==mail);
        }

        public User GetByMailComfirmValue(string value)
        {
            return _userDal.Get(p=> p.MailConfirmValue==value);
        }

        public List<OperationClaim> GetClaims(User user, int companyId  )
        {
            return _userDal.GetClaims(user, companyId);
        }

        public void Update(User user)
        {
           _userDal.Update(user);

        }
    }
}
