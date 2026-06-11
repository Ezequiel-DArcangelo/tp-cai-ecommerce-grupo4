using Cart.API.DTOs;
using Cart.API.Exceptions;
using Cart.API.Models;

namespace Cart.API.Services
{
    public class CartService
    {
        // Diccionario en memoria que simula la base de datos: userId -> Cart
        private static Dictionary<Guid, Cart.API.Models.Cart> _carts = new Dictionary<Guid, Cart.API.Models.Cart>();

        // GET /api/cart/{userId}
        public CartResponse GetCart(Guid userId)
        {
            if (!_carts.TryGetValue(userId, out var cart))
                throw new NotFoundException("CRT-001", $"El usuario con ID {userId} no tiene un carrito activo.");

            return MapToResponse(cart);
        }

        // POST /api/cart/{userId}/items
        public CartResponse AddItem(Guid userId, AddItemRequest request)
        {
            ValidarCantidad(request.Cantidad);
            ValidarProductoId(request.ProductoId);

            if (!_carts.TryGetValue(userId, out var cart))
            {
                // Si no tiene carrito, se crea uno nuevo
                cart = new Cart.API.Models.Cart
                {
                    UsuarioId = userId,
                    Items = new List<CartItemDTO>(),
                    FechaActualizacion = DateTime.UtcNow
                };
                _carts[userId] = cart;
            }

            var itemExistente = cart.Items.FirstOrDefault(i => i.ProductoId == request.ProductoId);
            if (itemExistente != null)
            {
                // Si el producto ya está en el carrito, se suma la cantidad
                itemExistente.Cantidad += request.Cantidad;
            }
            else
            {
                cart.Items.Add(new CartItemDTO
                {
                    ProductoId = request.ProductoId,
                    Cantidad = request.Cantidad
                });
            }

            cart.FechaActualizacion = DateTime.UtcNow;
            return MapToResponse(cart);
        }

        // PUT /api/cart/{userId}/items/{productId}
        public CartResponse UpdateItem(Guid userId, Guid productId, UpdateItemRequest request)
        {
            ValidarCantidad(request.Cantidad);
            ValidarProductoId(productId);

            if (!_carts.TryGetValue(userId, out var cart))
                throw new NotFoundException("CRT-001", $"El usuario con ID {userId} no tiene un carrito activo.");

            var item = cart.Items.FirstOrDefault(i => i.ProductoId == productId);
            if (item == null)
                throw new NotFoundException("CRT-002", $"El producto con ID {productId} no existe en el carrito.");

            item.Cantidad = request.Cantidad;
            cart.FechaActualizacion = DateTime.UtcNow;
            return MapToResponse(cart);
        }

        // DELETE /api/cart/{userId}/items/{productId}
        public void RemoveItem(Guid userId, Guid productId)
        {
            if (!_carts.TryGetValue(userId, out var cart))
                throw new NotFoundException("CRT-001", $"El usuario con ID {userId} no tiene un carrito activo.");

            var item = cart.Items.FirstOrDefault(i => i.ProductoId == productId);
            if (item == null)
                throw new NotFoundException("CRT-002", $"El producto con ID {productId} no existe en el carrito.");

            cart.Items.Remove(item);
            cart.FechaActualizacion = DateTime.UtcNow;
        }

        // DELETE /api/cart/{userId}
        public void ClearCart(Guid userId)
        {
            if (!_carts.ContainsKey(userId))
                throw new NotFoundException("CRT-001", $"El usuario con ID {userId} no tiene un carrito activo.");

            _carts.Remove(userId);
        }

        // Auxiliar: convertir Cart en CartResponse
        private CartResponse MapToResponse(Cart.API.Models.Cart cart)
        {
            return new CartResponse
            {
                UsuarioId = cart.UsuarioId,
                Items = cart.Items,
                FechaActualizacion = cart.FechaActualizacion
            };
        }

        // Auxiliar: validar que la cantidad sea mayor a 0 (CRT-004)
        private void ValidarCantidad(int cantidad)
        {
            if (cantidad <= 0)
                throw new ValidationException("CRT-004", "La cantidad debe ser mayor a cero.");
        }

        // Auxiliar: simular validación de producto en Products API (CRT-002)
        private void ValidarProductoId(Guid productoId)
        {
            if (productoId == Guid.Empty)
                throw new NotFoundException("CRT-002", $"El producto con ID {productoId} no existe.");
        }
    }
}