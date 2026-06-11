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

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(Guid userId)
        {
            var response = await _cartService.GetCartAsync(userId);
            return Ok(response);
        }

        [HttpPost("{userId}/items")]
        public async Task<IActionResult> AddItem(Guid userId, [FromBody] AddItemRequest request)
        {
            var response = await _cartService.AddItemAsync(userId, request);
            return Ok(response);
        }

        [HttpPut("{userId}/items/{productId}")]
        public async Task<IActionResult> UpdateItem(Guid userId, string productId, [FromBody] UpdateItemRequest request)
        {
            var response = await _cartService.UpdateItemAsync(userId, productId, request);
            return Ok(response);
        }

        [HttpDelete("{userId}/items/{productId}")]
        public async Task<IActionResult> RemoveItem(Guid userId, string productId)
        {
            await _cartService.RemoveItemAsync(userId, productId);
            return NoContent();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> ClearCart(Guid userId)
        {
            await _cartService.ClearCartAsync(userId);
            return NoContent();
        }
    }
}