using Microsoft.AspNetCore.Mvc;
using Cart.API.DTOs;
using Cart.API.Services;

namespace Cart.API.Controllers
{
    /// <summary>
    /// Gestión del carrito de compras
    /// </summary>
    [ApiController]
    [Route("api/cart")]
    [Tags("Cart")]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>Obtener el carrito de un usuario</summary>
        /// <param name="userId">ID del usuario</param>
        /// <response code="200">Carrito encontrado</response>
        /// <response code="404">CRT-001: El usuario no tiene un carrito activo</response>
        /// <response code="500">CRT-005: Error interno</response>
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(CartResponse), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCart(Guid userId)
        {
            var response = await _cartService.GetCartAsync(userId);
            return Ok(response);
        }

        /// <summary>Agregar un producto al carrito</summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="request">Producto y cantidad a agregar</param>
        /// <response code="200">Producto agregado correctamente</response>
        /// <response code="400">CRT-004: Cantidad inválida</response>
        /// <response code="404">CRT-002: Producto no encontrado</response>
        /// <response code="422">CRT-003: Stock insuficiente</response>
        /// <response code="500">CRT-005: Error interno</response>
        [HttpPost("{userId}/items")]
        [ProducesResponseType(typeof(CartResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AddItem(Guid userId, [FromBody] AddItemRequest request)
        {
            var response = await _cartService.AddItemAsync(userId, request);
            return Ok(response);
        }

        /// <summary>Actualizar la cantidad de un producto en el carrito</summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="productId">ID del producto</param>
        /// <param name="request">Nueva cantidad</param>
        /// <response code="200">Cantidad actualizada correctamente</response>
        /// <response code="400">CRT-004: Cantidad inválida</response>
        /// <response code="404">CRT-001: Carrito no encontrado | CRT-002: Producto no encontrado en el carrito</response>
        /// <response code="422">CRT-003: Stock insuficiente</response>
        /// <response code="500">CRT-005: Error interno</response>
        [HttpPut("{userId}/items/{productId}")]
        [ProducesResponseType(typeof(CartResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateItem(Guid userId, string productId, [FromBody] UpdateItemRequest request)
        {
            var response = await _cartService.UpdateItemAsync(userId, productId, request);
            return Ok(response);
        }

        /// <summary>Quitar un producto del carrito</summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="productId">ID del producto a quitar</param>
        /// <response code="204">Producto eliminado correctamente</response>
        /// <response code="404">CRT-001: Carrito no encontrado | CRT-002: Producto no encontrado en el carrito</response>
        /// <response code="500">CRT-005: Error interno</response>
        [HttpDelete("{userId}/items/{productId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RemoveItem(Guid userId, string productId)
        {
            await _cartService.RemoveItemAsync(userId, productId);
            return NoContent();
        }

        /// <summary>Vaciar el carrito completo de un usuario</summary>
        /// <param name="userId">ID del usuario</param>
        /// <response code="204">Carrito vaciado correctamente</response>
        /// <response code="404">CRT-001: El usuario no tiene un carrito activo</response>
        /// <response code="500">CRT-005: Error interno</response>
        [HttpDelete("{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ClearCart(Guid userId)
        {
            await _cartService.ClearCartAsync(userId);
            return NoContent();
        }
    }
}