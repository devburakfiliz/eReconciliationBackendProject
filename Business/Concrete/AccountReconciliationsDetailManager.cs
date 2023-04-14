using Business.Abstract;
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
    public class AccountReconciliationsDetailManager : IAccountReconciliationsDetailService
    {
        private readonly IAccountReconciliationsDetailDal _accountReconciliationsDetailDal;

        public AccountReconciliationsDetailManager(IAccountReconciliationsDetailDal accountReconciliationsDetailDal)
        {
            _accountReconciliationsDetailDal = accountReconciliationsDetailDal;
        }
        [CacheRemoveAspect("IAccountReconciliationsDetailService.Get")]
        public IResult Add(AccountReconciliationsDetail accountReconciliationDetail)
        {
            _accountReconciliationsDetailDal.Add(accountReconciliationDetail);
            return new SuccessResult("Cari Mütabakat Detay Bilgisi Eklendi");
        }
        [CacheRemoveAspect("IAccountReconciliationsDetailService.Get")]
        [TransactionScopeAspect]
        public IResult AddToExcel(string filePath, int accountReconciliationId)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        string description = reader.GetString(1);



                        if (description != "Açıklama" && description != null)
                        {
                            DateTime date = reader.GetDateTime(0);
                            double currencyId = reader.GetDouble(2);
                            double debit = reader.GetDouble(3);
                            double credit = reader.GetDouble(4);


                            AccountReconciliationsDetail accountReconciliationsDetail = new AccountReconciliationsDetail()
                            {
                                AccountReconciliationId = accountReconciliationId,
                                Description = description,
                                Date = date,
                                CurrencyCredit = Convert.ToDecimal(credit),
                                CurrencyDebit = Convert.ToDecimal(debit),
                                CurrencyId = Convert.ToInt16(currencyId)
                            };
                             _accountReconciliationsDetailDal.Add(accountReconciliationsDetail);
                        }
                    }

                }

            }
            File.Delete(filePath);
            return new SuccessResult("Exceldeki cariler eklendi");
        }
        [CacheRemoveAspect("IAccountReconciliationsDetailService.Get")]
        public IResult Delete(AccountReconciliationsDetail accountReconciliationDetail)
        {
            _accountReconciliationsDetailDal.Delete(accountReconciliationDetail);
            return new SuccessResult("Cari Mütabakat Detay Bilgisi silindi");
        }
        [CacheAspect(60)]
        public IDataResult<AccountReconciliationsDetail> GetById(int id)
        {
            return new SuccessDataResult<AccountReconciliationsDetail>(_accountReconciliationsDetailDal.Get(p=>p.Id == id));
        }
        [CacheAspect(60)]
        public IDataResult<List<AccountReconciliationsDetail>> GetList(int accountReconciliationId)
        {
            return new SuccessDataResult<List<AccountReconciliationsDetail>>(_accountReconciliationsDetailDal.GetList(p => p.AccountReconciliationId == accountReconciliationId));

        }
        [CacheRemoveAspect("IAccountReconciliationsDetailService.Get")]
        public IResult Update(AccountReconciliationsDetail accountReconciliationDetail)
        {
            _accountReconciliationsDetailDal.Update(accountReconciliationDetail);
            return new SuccessResult("Cari Mütabakat Detay Bilgisi güncellendi");
        }
    }
}
