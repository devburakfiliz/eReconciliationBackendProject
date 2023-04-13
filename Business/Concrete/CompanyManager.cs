using Business.Abstract;
using Business.Constans;
using Business.ValidationRule.FluentValidation;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.Entities.Concrete;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class CompanyManager : ICompanyService
    {
        private readonly ICompanyDal _companyDal;

        public CompanyManager(ICompanyDal companyDal)
        {
            _companyDal = companyDal;
        }

        [ValidationAspect(typeof(CompanyValidator))]

        public IResult Add(Company company)
        {
            _companyDal.Add(company);
            return new SuccessResult(Messages.AddedCompany);
        }


        [ValidationAspect(typeof(CompanyValidator))]
        [TransactionScopeAspect]
        public IResult CompanyAndUserCompany(CompanyDto companyDto)
        {
            _companyDal.Add(companyDto.Company);
            _companyDal.UserCompanyAdd(companyDto.UserId, companyDto.Company.Id);
            return new SuccessResult(Messages.AddedCompany);
        }

        public IResult CompanyExist(Company company)
        {
            var result = _companyDal.Get(c=>c.Name == company.Name && c.TaxDepartment == company.TaxDepartment &&
            c.TaxIdNumber == company.TaxIdNumber && c.IdentityNumber == company.IdentityNumber);

            if (result != null)
            {
                return new ErrorResult("Bu şirket daha önce kayıt edilmiş.");
            }
            return new SuccessResult();
        }

        public IDataResult<Company> GetById(int id)
        {
            return new SuccessDataResult<Company>(_companyDal.Get(P=>P.Id == id));
        }

        public IDataResult<UserCompany> GetCompany(int userId)
        {
            return new SuccessDataResult<UserCompany>(_companyDal.GetCompany(userId));
        }

        public IDataResult<List<Company>> GetList()
        {
            return new SuccessDataResult<List<Company>>(_companyDal.GetList(), Messages.GetCompanies);
        }

        public IResult Update(Company company)
        {
            _companyDal.Update(company);
            return new SuccessResult(Messages.UpdatedCompany);
        }

        public IResult UserCompanyAdd(int userId, int companyId)
        {
            _companyDal.UserCompanyAdd(userId, companyId);
            return new SuccessResult();
        }
    }
}
