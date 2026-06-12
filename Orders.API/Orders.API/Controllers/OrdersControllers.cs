using Microsoft.AspNetCore.Mvc;
using Orders.API.DTOs;
using Orders.API.Services;

namespace Orders.API.Controllers
{
    /// <summary>
    /// Gestión de órdenes de compra
    /// </summary>
    [ApiController]
    [Route("api/orders")]
    [Tags("Orders")]
    public class OrdersController : ControllerBase
    {
        private readonly OrdersService _ordersService;

        public OrdersController(OrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        /// <summary>Listar todas las órdenes con filtro opcional por usuario</summary>
        /// <param name="usuarioId">ID del usuario para filtrar (opcional)</param>
        /// <response code="200">Lista de órdenes</response>
        /// <response code="500">ORD-007: Error interno</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<OrderResponse>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetOrders([FromQuery] string? usuarioId)
        {
            var response = await _ordersService.GetOrders(usuarioId);
            return Ok(response);
        }

        /// <summary>Obtener detalle de una orden por ID</summary>
        /// <param name="id">ID de la orden</param>
        /// <response code="200">Orden encontrada</response>
        /// <response code="404">ORD-001: Orden no encontrada</response>
        /// <response code="500">ORD-007: Error interno</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderResponse), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetOrderById(string id)
        {
            var response = await _ordersService.GetOrderById(id);
            return Ok(response);
        }

        /// <summary>Crear una nueva orden</summary>
        /// <param name="request">Datos de la orden</param>
        /// <response code="201">Orden creada correctamente</response>
        /// <response code="400">ORD-002: Datos de la orden inválidos</response>
        /// <response code="404">ORD-003: Usuario no encontrado | ORD-004: Producto no encontrado</response>
        /// <response code="422">ORD-005: Stock insuficiente</response>
        /// <response code="500">ORD-007: Error interno</response>
        [HttpPost]
        [ProducesResponseType(typeof(OrderResponse), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var response = await _ordersService.CreateOrder(request);
            return StatusCode(201, response);
        }

        /// <summary>Actualizar el estado de una orden</summary>
        /// <param name="id">ID de la orden</param>
        /// <param name="request">Nuevo estado</param>
        /// <response code="200">Estado actualizado correctamente</response>
        /// <response code="404">ORD-001: Orden no encontrada</response>
        /// <response code="409">ORD-006: Transición de estado inválida</response>
        /// <response code="500">ORD-007: Error interno</response>
        [HttpPut("{id}/status")]
        [ProducesResponseType(typeof(OrderResponse), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] UpdateOrderStatusRequest request)
        {
            var response = await _ordersService.UpdateOrderStatus(id, request);
            return Ok(response);
        }

        /// <summary>Verificar si un producto tiene órdenes activas</summary>
        /// <param name="id">ID del producto</param>
        /// <response code="200">Resultado de la verificación</response>
        /// <response code="500">ORD-007: Error interno</response>
        [HttpGet("check-product/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CheckProductActiveOrders(string id)
        {
            bool tieneOrdenes = await _ordersService.ExisteProductoEnOrdenesActivas(id);
            return Ok(new { TieneOrdenesActivas = tieneOrdenes });
        }
    }
}