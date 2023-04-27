using Business.Abstract;
using Core.Utilities.Results;
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

        [HttpGet()]
        public IActionResult GetParities()
        {
            var result = _parityService.GetParities();
            return Ok(result.Data);
        }

        [HttpGet("getpricebyid")]
        public async Task<IActionResult> GetCryptoPrice(int id) 
        {
            var result = await _parityService.GetPrice(id);
            return Ok(result);
        }

        [HttpPost("buy")]
        public IActionResult BuyCrypto(BuyCryptoDto buyCryptoDto)
        {
            var result = _cryptoService.BuyCrypto(buyCryptoDto).Result;
            return Ok(result);
        }

        [HttpPost("sell")]
        public IActionResult SellCrypto(SellCryptoDto sellCryptoDto)
        {
            var result = _cryptoService.SellCrypto(sellCryptoDto);
            return Ok(result);
        }


    }
}
