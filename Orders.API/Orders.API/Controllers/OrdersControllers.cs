using Microsoft.AspNetCore.Mvc;
using Orders.API.DTOs;
using Orders.API.Services;

namespace Orders.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly OrdersService _ordersService;

        public OrdersController(OrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        // GET /api/orders?usuarioId=123
        [HttpGet]
        public IActionResult GetOrders([FromQuery] int? usuarioId)
        {
            var response = _ordersService.GetOrders(usuarioId);
            return Ok(response); // 200 OK
        }

        // GET /api/orders/{id}
        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            var response = _ordersService.GetOrderById(id);
            return Ok(response); // 200 OK (si no existe, lanza ORD-001 y el handler devuelve 404)
        }

        // POST /api/orders
        [HttpPost]
        public IActionResult CreateOrder([FromBody] CreateOrderRequest request)
        {
            var response = _ordersService.CreateOrder(request);
            return StatusCode(201, response); // 201 Created
        }

        // PUT /api/orders/{id}/status
        [HttpPut("{id}/status")]
        public IActionResult UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
        {
            var response = _ordersService.UpdateOrderStatus(id, request);
            return Ok(response); // 200 OK (si transición inválida, lanza ORD-006 y el handler devuelve 409)
        }
    }
}