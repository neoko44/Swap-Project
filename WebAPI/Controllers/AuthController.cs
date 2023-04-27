
using Business.Abstract;
using Core.Utilities.Security.Jwt;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("register")]
        public IActionResult Register(RegisterDto registerDto)
        {
            var result = _authService.Register(registerDto);
            return Ok(result);
        }


        [HttpPost("login")]
        public IActionResult LoginMail(EmailLoginDto emailLoginDto)
        {
            var userToLogin = _authService.LoginMail(emailLoginDto.Email, emailLoginDto.Password);
            if (!userToLogin.Success)
            {
                return BadRequest(userToLogin.Message);
            }

            var result = _authService.CreateAccessToken(userToLogin.Data);
            return Ok(result);
        }


        [HttpPost("updatepassword")]
        public IActionResult ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var result = _authService.ChangePassword(changePasswordDto);

            return Ok(result);
        }




        //[HttpPost("withdraw")]
        //public IActionResult WithdrawBalance(Deposit deposit)
        //{
        //    var result = _walletService.Withdraw(deposit);

        //    if (!result.Success)
        //    {
        //        return BadRequest(result.Message);
        //    }
        //    return Ok(result.);
        //}

    }
}
