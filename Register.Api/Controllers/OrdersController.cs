using Microsoft.AspNetCore.Mvc;
using Register.Application.DTOs;
using Register.Application.Services;

namespace Register.Api.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrdersController(IOrderService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> Post(CreateOrderDto order)
        {
            if (!ModelState.IsValid)
                return BadRequest($"Preencha todos os dados");

            await _service.Add(order);

            Console.WriteLine($"Add com sucesso {order.Status}");

            return Ok(order);
        }
    }
}
