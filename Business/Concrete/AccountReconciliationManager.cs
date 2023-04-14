using Business.Abstract;
using Business.Constans;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Caching;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class AccountReconciliationManager : IAccountReconciliationService
    {
        private readonly IAccountReconciliationDal _accountReconciliationDal;
        private readonly ICurrencyAccountService _currencyAccountService;

        public AccountReconciliationManager(IAccountReconciliationDal accountReconciliationDal, ICurrencyAccountService currencyAccountService)
        {
            _accountReconciliationDal = accountReconciliationDal;
            _currencyAccountService = currencyAccountService;
        }

        [CacheRemoveAspect("IAccountReconciliationService.Get")]
        public IResult Add(AccountReconciliation accountReconciliation)
        {
            _accountReconciliationDal.Add(accountReconciliation);
            return new SuccessResult(Messages.AddedAccountReconciliation);
        }
        [CacheRemoveAspect("IAccountReconciliationService.Get")]
        [TransactionScopeAspect]
        public IResult AddToExcel(string filePath, int companyId)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        string code = reader.GetString(0);



                        if (code != "Cari Kodu" && code != null)
                        {
                            DateTime startingDate = reader.GetDateTime(1);
                            DateTime endingDate = reader.GetDateTime(2);
                            double currencyId = reader.GetDouble(3);
                            double debit = reader.GetDouble(4);
                            double credit = reader.GetDouble(5);

                            int currencyAccountId = _currencyAccountService.GetByCode(code, companyId).Data.Id;
                            string guid = Guid.NewGuid().ToString();

                            AccountReconciliation accountReconciliation = new AccountReconciliation()
                            {
                                CompanyId = companyId,
                                CurrentAccountId = currencyAccountId,
                                CurrencyCredit = Convert.ToDecimal(credit),
                                CurrencyDebit = Convert.ToDecimal(debit),
                                CurrencyId = Convert.ToInt16(currencyId),
                                StartingDate = startingDate,
                                EndingDate = endingDate,
                            };
                            _accountReconciliationDal.Add(accountReconciliation);
                        }
                    }

                }

            }
            File.Delete(filePath);
            return new SuccessResult("Exceldeki cariler eklendi");
        }


        [CacheRemoveAspect("IAccountReconciliationService.Get")]
        public IResult Delete(AccountReconciliation accountReconciliation)
        {
            _accountReconciliationDal.Delete(accountReconciliation);
            return new SuccessResult(Messages.DeletedAccountReconciliation);
        }

        public IDataResult<AccountReconciliation> GetById(int id)
        {
            return new SuccessDataResult<AccountReconciliation>(_accountReconciliationDal.Get(p => p.Id == id));
        }

        [CacheAspect(60)]
        public IDataResult<List<AccountReconciliation>> GetList(int companyId)
        {
            return new SuccessDataResult<List<AccountReconciliation>>(_accountReconciliationDal.GetList(p => p.CompanyId == companyId));
        }
        [CacheRemoveAspect("IAccountReconciliationService.Get")]
        public IResult Update(AccountReconciliation accountReconciliation)
        {
            _accountReconciliationDal.Update(accountReconciliation);
            return new SuccessResult(Messages.UpdatedAccountReconciliation);
        }
    }
}
