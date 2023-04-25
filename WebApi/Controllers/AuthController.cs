using Business.Abstract;
using Core.Utilities.Results.Concrete;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("registerSecondAccount")]
        public IActionResult RegisterSecondAccount(UserForRegisterToSecondAccountDto userForRegister, int companyId)
        {
            var userExists = _authService.UserExists(userForRegister.Email);
            if (!userExists.Success)
            {
                return BadRequest(userExists.Message);
            }
            var registerResult = _authService.RegisterSecondAccount(userForRegister, userForRegister.Password, userForRegister.CompanyId);
            var result = _authService.CreateAccessToken(registerResult.Data,userForRegister.CompanyId);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(registerResult.Message);
        }


        [HttpPost("register")]
        public IActionResult Register(UserAndCompanyRegisterDto userAndCompanyRegister)
        {
            var userExists = _authService.UserExists(userAndCompanyRegister.userForRegister.Email);
            if (!userExists.Success)
            {
                return BadRequest(userExists.Message);
            }

            var companyExists = _authService.CompanyExists(userAndCompanyRegister.company);
            if (!companyExists.Success)
            {
                return BadRequest(companyExists.Message);
            }

            var registerResult = _authService.Register(userAndCompanyRegister.userForRegister,userAndCompanyRegister.userForRegister.Password,userAndCompanyRegister.company);
            var result = _authService.CreateAccessToken(registerResult.Data,registerResult.Data.CompanyId);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(registerResult.Message);
        }
        [HttpPost("login")]
        public IActionResult Login(UserForLogin userForLogin)
        {
            var userToLogin = _authService.Login(userForLogin);

            if (!userToLogin.Success)
            {
                return BadRequest(userToLogin.Message);
            }

            if (userToLogin.Data.IsActive)
            {

                var userCompany = _authService.GetCompany(userToLogin.Data.Id).Data;
                var result = _authService.CreateAccessToken(userToLogin.Data, userCompany.CompanyId ); 
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result);

            }

                return BadRequest("Kullanıcı Pasif Durumda Pasif kullanıcılar sisteme giriş yapamaz  (Yöneticinize danışın)");




        }
        [HttpGet("confirmuser")]
        public IActionResult ComfirmUser(string value) 
        {
            var user = _authService.GetByMailConfirmValue(value).Data;
            user.MailConfirm = true;
            user.MailConfirmDate = DateTime.Now;
            var result =_authService.Update(user);
            if (result.Success)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet("sendconfirmEmail")]
        public IActionResult SendConfirmEmail(int id)
        {
            var user = _authService.GetById(id).Data;
            var result =_authService.SendConfirmEmail(user);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
    }
}
