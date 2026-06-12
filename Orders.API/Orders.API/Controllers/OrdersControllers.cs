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

        // GET /api/orders?usuarioId=abc
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] string? usuarioId)
        {
            var response = await _ordersService.GetOrders(usuarioId);
            return Ok(response);
        }

        // GET /api/orders/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(string id)
        {
            var response = await _ordersService.GetOrderById(id);
            return Ok(response);
        }

        // POST /api/orders
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var response = await _ordersService.CreateOrder(request);
            return StatusCode(201, response);
        }

        // PUT /api/orders/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] UpdateOrderStatusRequest request)
        {
            var response = await _ordersService.UpdateOrderStatus(id, request);
            return Ok(response);
        }

        // GET /api/orders/check-product/{productId}
        [HttpGet("check-product/{id}")]
        public async Task<IActionResult> CheckProductActiveOrders(string id)
        {
            bool tieneOrdenes = await _ordersService.ExisteProductoEnOrdenesActivas(id);
            return Ok(new { TieneOrdenesActivas = tieneOrdenes });
        }
    }
}