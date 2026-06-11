using Microsoft.AspNetCore.Mvc;
using Cart.API.DTOs;
using Cart.API.Services;

namespace Cart.API.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        // GET /api/cart/{userId}
        [HttpGet("{userId}")]
        public IActionResult GetCart(Guid userId)
        {
            var response = _cartService.GetCart(userId);
            return Ok(response); // 200 OK (si no existe, lanza CRT-001 y el handler devuelve 404)
        }

        // POST /api/cart/{userId}/items
        [HttpPost("{userId}/items")]
        public IActionResult AddItem(Guid userId, [FromBody] AddItemRequest request)
        {
            var response = _cartService.AddItem(userId, request);
            return Ok(response); // 200 OK
        }

        // PUT /api/cart/{userId}/items/{productId}
        [HttpPut("{userId}/items/{productId}")]
        public IActionResult UpdateItem(Guid userId, Guid productId, [FromBody] UpdateItemRequest request)
        {
            var response = _cartService.UpdateItem(userId, productId, request);
            return Ok(response); // 200 OK
        }

        // DELETE /api/cart/{userId}/items/{productId}
        [HttpDelete("{userId}/items/{productId}")]
        public IActionResult RemoveItem(Guid userId, Guid productId)
        {
            _cartService.RemoveItem(userId, productId);
            return NoContent(); // 204 No Content
        }

        // DELETE /api/cart/{userId}
        [HttpDelete("{userId}")]
        public IActionResult ClearCart(Guid userId)
        {
            _cartService.ClearCart(userId);
            return NoContent(); // 204 No Content
        }
    }
}