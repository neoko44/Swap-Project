using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        IWalletService _walletService;

        public UserController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpPost("deposit")]
        public IActionResult DepositBalance(Deposit deposit)
        {
            var result = _walletService.Deposit(deposit);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
        [HttpGet("getwalletlist")]
        public IActionResult GetWallets()
        {
            var result = _walletService.GetWallets();
            return Ok(result);
        }

        
    }
}
