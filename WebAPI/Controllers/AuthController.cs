
using Business.Abstract;
using Core.Utilities.Security.Jwt;
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
        IWalletService _walletService;

        public AuthController(IAuthService authService, IWalletService walletService)
        {
            _authService = authService;
            _walletService = walletService;
        }



        [HttpPost("register")]
        public IActionResult Register(RegisterDto registerDto)
        {
            var result = _authService.Register(registerDto);
            return Ok(result);
        }


        [HttpPost("login/mail")]
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


        [HttpPost("update/password")]
        public IActionResult ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var result = _authService.ChangePassword(changePasswordDto);

            return Ok(result);
        }


        [HttpPost("deposit/balance")]
        public IActionResult DepositBalance(decimal money)
        {
            var result = _walletService.Deposit(money);
            return Ok(result);
        }

    }
}
