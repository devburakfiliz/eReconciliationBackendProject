using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountReconciliationDetailController : ControllerBase
    {
        private readonly IAccountReconciliationsDetailService _accountReconciliationsDetailService;

        public AccountReconciliationDetailController(IAccountReconciliationsDetailService accountReconciliationsDetailService)
        {
            _accountReconciliationsDetailService = accountReconciliationsDetailService;
        }

        [HttpPost("add")]
        public IActionResult Add(AccountReconciliationsDetail accountReconciliationDetail)
        {
            var result = _accountReconciliationsDetailService.Add(accountReconciliationDetail);
            if (result.Success)
            {
                return Ok(result);

            }
            return BadRequest(result.Message);
        }
        [HttpPost("update")]
        public IActionResult Update(AccountReconciliationsDetail accountReconciliationDetail)
        {
            var result = _accountReconciliationsDetailService.Update(accountReconciliationDetail);
            if (result.Success)
            {
                return Ok(result);

            }
            return BadRequest(result.Message);
        }
        [HttpPost("delete")]
        public IActionResult Delete(AccountReconciliationsDetail accountReconciliationDetail)
        {
            var result = _accountReconciliationsDetailService.Delete(accountReconciliationDetail);
            if (result.Success)
            {
                return Ok(result);

            }
            return BadRequest(result.Message);
        }
        [HttpGet("getById")]
        public IActionResult GetById(int id)
        {
            var result = _accountReconciliationsDetailService.GetById(id);
            if (result.Success)
            {
                return Ok(result);

            }
            return BadRequest(result.Message);
        }
        [HttpGet("getList")]
        public IActionResult GetList(int accountReconciliationId)
        {
            var result = _accountReconciliationsDetailService.GetList(accountReconciliationId);
            if (result.Success)
            {
                return Ok(result);

            }
            return BadRequest(result.Message);
        }
        [HttpPost("addFromExcel")]
        public IActionResult AddFromExcel(IFormFile file, int accountReconciliationId)
        {
            if (file.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + ".xlsx";
                var filePath = $"{Directory.GetCurrentDirectory()}/Content/{fileName}";
                using (FileStream stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                    stream.Flush();
                }
                var result = _accountReconciliationsDetailService.AddToExcel(filePath, accountReconciliationId);
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result.Message);

            }
            return BadRequest("Dosya Seçimi yapmadınız");

        }
    }
}
