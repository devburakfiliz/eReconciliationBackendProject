using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Context;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfAccountReconciliationDal : EfEntityRepositoryBase<AccountReconciliation, ContextDb>, IAccountReconciliationDal
    {
        public List<AccountReconcliationDto> GetAllDto(int companyId)
        {
            using (var context = new ContextDb())
            {
                var result = from reconciliation in context.AccountReconciliations.Where(p => p.CompanyId == companyId)
                             join company in context.Companies on reconciliation.CompanyId equals company.Id
                             join account in context.CurrencyAccounts on reconciliation.CurrentAccountId equals account.Id
                             join currency in context.Currencies on reconciliation.CurrencyId equals currency.Id
                             select new AccountReconcliationDto
                             {
                                 CompanyId = companyId,
                                 CurrentAccountId = account.Id,
                                 AccountIdentityNumber = account.IdentityNumber,
                                 AccountName = account.Name,
                                 AccountTaxDepartment = account.TaxDepartment,
                                 AccountTaxNumber = account.IdentityNumber,
                                 CompanyIdentityNumber = company.IdentityNumber,
                                 CompanyName = account.Name,
                                 CompanyTaxDepartment = company.TaxDepartment,
                                 CompanyTaxNumber = company.TaxIdNumber,
                                 CurrencyCredit = reconciliation.CurrencyCredit,
                                 CurrencyDebit = reconciliation.CurrencyDebit,
                                 EmailReadDate = reconciliation.EmailReadDate,
                                 EndingDate = reconciliation.EndingDate,
                                 Guid = reconciliation.Guid,
                                 Id = reconciliation.Id,
                                 IsEmailRead = reconciliation.IsEmailRead,
                                 IsResultSucceed = reconciliation.IsResultSucceed,
                                 ResultDate = reconciliation.ResultDate,
                                 ResultNote = reconciliation.ResultNote,
                                 SendEmailDate = reconciliation.SendEmailDate,
                                 StartingDate = reconciliation.StartingDate,
                                 CurrencyCode = currency.Code,
                                 AccountEMail =account.Email
                             };
                return result.ToList();

            }
        }
    }
}
