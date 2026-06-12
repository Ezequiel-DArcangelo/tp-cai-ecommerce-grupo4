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

        /// <summary>Constructor</summary>
        public OrdersController(OrdersService ordersService)
        {
            _ordersService = ordersService;
        }
    } }
        /// <summary>Listar todas las órdenes con filtro opcional por usuario</summary>
        /// <param name="usuarioId">ID del usuario para filtrar (opcional)</param>
        /// <response code="200">Lista de órdenes</response>
        /// <response code="500">ORD-007: Error intern