using Business.Abstract;
using Entities.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CryptoController : ControllerBase
    {
        IParityService _parityService;
        ICryptoService _cryptoService;

        public CryptoController(IParityService parityService, ICryptoService cryptoService)
        {
            _parityService = parityService;
            _cryptoService = cryptoService;
        }

        [HttpGet("get/crypto/names")]
        public IActionResult GetParities()
        {
            var result = _parityService.GetParities();
            return Ok(result.Data);
        }

        [HttpGet("get/crypto/price")]
        public IActionResult GetCryptoPrice(int id) 
        {
            var result = _parityService.GetPrice(id);
            return Ok(result.Result.Data);
        }

        [HttpPost("buy/crypto")]
        public IActionResult BuyCrypto(BuyCryptoDto buyCryptoDto)
        {
            var result = _cryptoService.BuyCrypto(buyCryptoDto);
            return Ok(result);
        }
    }
}
