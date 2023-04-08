using Business.Abstract;
using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class AccountReconciliationsDetailManager : IAccountReconciliationsDetailService
    {
        private readonly IAccountReconciliationsDetailDal _accountReconciliationsDetailDal;

        public AccountReconciliationsDetailManager(IAccountReconciliationsDetailDal accountReconciliationsDetailDal)
        {
            _accountReconciliationsDetailDal = accountReconciliationsDetailDal;
        }
    }
}
