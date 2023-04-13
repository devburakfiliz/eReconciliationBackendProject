using Business.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : Controller
    { 
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet("getcompanylist")]
        public IActionResult GetCompanyList()
        {
            var result = _companyService.GetList();
            if (result.Success)
            {
                    return Ok(result);
            }
            return BadRequest();
        }
        [HttpGet("getcompany")]
        public IActionResult GetCompany(int id)
        {
            var result = _companyService.GetById(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("updateCompany")]
        public IActionResult UpdateCompanyAndUserCompany(Company company)
        {
            var result = _companyService.Update(company);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }


        [HttpPost("addcompany")]
        public IActionResult AddCompany(CompanyDto company)
        {
            var result = _companyService.Add(company.Company);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest();
        }
        [HttpPost("addCompanyAndUserCompany")]
        public IActionResult AddCompanyAndUserCompany(CompanyDto company)
        {
            var result = _companyService.CompanyAndUserCompany(company);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest();
        }
    }
}
