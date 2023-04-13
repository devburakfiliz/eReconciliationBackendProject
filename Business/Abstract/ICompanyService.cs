using Core.Entities.Concrete;
using Core.Utilities.Results.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public  interface ICompanyService
    {
        IDataResult<List<Company>> GetList();
        IDataResult<UserCompany> GetCompany(int userId);
        IResult Add (Company company);  
        IResult Update (Company company);  
        IDataResult<Company> GetById (int id);
        IResult CompanyAndUserCompany (CompanyDto companyDto);  
        IResult CompanyExist (Company company);
        IResult UserCompanyAdd(int userId, int companyId);
    }
}
