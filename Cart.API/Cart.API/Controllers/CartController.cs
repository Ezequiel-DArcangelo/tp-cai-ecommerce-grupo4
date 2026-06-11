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
        public async Task<IActionResult> GetCart(Guid userId)
        {
            var response = await _cartService.GetCartAsync(userId);
            return Ok(response);
        }

        // POST /api/cart/{userId}/items
        [HttpPost("{userId}/items")]
        public async Task<IActionResult> AddItem(Guid userId, [FromBody] AddItemRequest request)
        {
            var response = await _cartService.AddItemAsync(userId, request);
            return Ok(response);
        }

        // PUT /api/cart/{userId}/items/{productId}
        [HttpPut("{userId}/items/{productId}")]
        public async Task<IActionResult> UpdateItem(Guid userId, Guid productId, [FromBody] UpdateItemRequest request)
        {
            var response = await _cartService.UpdateItemAsync(userId, productId, request);
            return Ok(response);
        }

        // DELETE /api/cart/{userId}/items/{productId}
        [HttpDelete("{userId}/items/{productId}")]
        public async Task<IActionResult> RemoveItem(Guid userId, Guid productId)
        {
            await _cartService.RemoveItemAsync(userId, productId);
            return NoContent();
        }

        // DELETE /api/cart/{userId}
        [HttpDelete("{userId}")]
        public async Task<IActionResult> ClearCart(Guid userId)
        {
            await _cartService.ClearCartAsync(userId);
            return NoContent();
        }
    }
}