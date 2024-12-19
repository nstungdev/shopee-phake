using Microsoft.AspNetCore.Mvc;
using OrderSolution.API.DTOs;
using OrderSolution.API.Services;

namespace OrderSolution.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(IOrderService orderService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            await orderService.CreateOrderAsync(request);
            return Ok();
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrder([FromRoute] string id)
        {
            var res = await orderService.GetOrderAsync(id);
            return Ok(res);
        }
    }
}
