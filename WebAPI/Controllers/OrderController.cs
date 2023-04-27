using Business.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("cancelbuyorderbyid")]
        public IActionResult CancelBuyOrderById(int orderId)
        {
            var result = _orderService.CancelBuyOrder(orderId);
            return Ok(result);
        }

        [HttpPost("cancelsellorderbyid")]
        public IActionResult CancelSellOrderById(int orderId)
        {
            var result = _orderService.CancelSellOrder(orderId);
            return Ok(result);
        }

        [HttpGet("orders")]
        public IActionResult GetOrders()
        {
            var result = _orderService.GetOrders();
            return Ok(result);
        }
    }
}
