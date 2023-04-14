using Core.Utilities.Results.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IAccountReconciliationsDetailService
    {
        IResult Add(AccountReconciliationsDetail accountReconciliationDetail);
        IResult AddToExcel(string filePath, int accountReconciliationId);
        IResult Update(AccountReconciliationsDetail accountReconciliationDetail);
        IResult Delete(AccountReconciliationsDetail accountReconciliationDetail);
        IDataResult<AccountReconciliationsDetail> GetById(int id);
        IDataResult<List<AccountReconciliationsDetail>> GetList(int accountReconciliationId);
    }
}
