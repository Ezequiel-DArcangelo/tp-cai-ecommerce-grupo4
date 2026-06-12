using Cart.API.Data;
using Cart.API.DTOs;
using Cart.API.Exceptions;

namespace Cart.API.Services
{
    public class CartService
    {
        private readonly CartRepository _repository;
        private readonly ProductsApiClient _productsApiClient;
        private readonly ILogger<CartService> _logger;

        public CartService(CartRepository repository, ProductsApiClient productsApiClient, ILogger<CartService> logger)
        {
            _repository = repository;
            _productsApiClient = productsApiClient;
            _logger = logger;
        }

        public async Task<CartResponse> GetCartAsync(Guid userId)
        {
            var cart = await _repository.GetCartAsync(userId);
            if (cart == null)
                throw new NotFoundException("CRT-001", $"El usuario con ID {userId} no tiene un carrito activo.");

            var items = await _repository.GetItemsAsync(userId);
            return MapToResponse(userId, items);
        }

        public async Task<CartResponse> AddItemAsync(Guid userId, AddItemRequest request)
        {
            ValidarCantidad(request.Cantidad);
            ValidarProductoId(request.ProductoId);

            // Validar que el producto existe y tiene stock suficiente
            var product = await _productsApiClient.GetProductAsync(request.ProductoId);
            if (product!.Stock < request.Cantidad)
                throw new BusinessRuleException("CRT-003",
                    $"Stock insuficiente. Disponible: {product.Stock}, solicitado: {request.Cantidad}.");

            await _repository.CreateCartAsync(userId);
            await _repository.AddOrUpdateItemAsync(userId, request.ProductoId, request.Cantidad);

            _logger.LogInformation("Producto {ProductoId} agregado al carrito del usuario {UserId}",
                request.ProductoId, userId);

            var items = await _repository.GetItemsAsync(userId);
            return MapToResponse(userId, items);
        }

        public async Task<CartResponse> UpdateItemAsync(Guid userId, string productId, UpdateItemRequest request)
        {
            ValidarCantidad(request.Cantidad);
            ValidarProductoId(productId);

            var cart = await _repository.GetCartAsync(userId);
            if (cart == null)
                throw new NotFoundException("CRT-001", $"El usuario con ID {userId} no tiene un carrito activo.");

            // Validar stock al actualizar cantidad
            var product = await _productsApiClient.GetProductAsync(productId);
            if (product!.Stock < request.Cantidad)
                throw new BusinessRuleException("CRT-003",
                    $"Stock insuficiente. Disponible: {product.Stock}, solicitado: {request.Cantidad}.");

            var updated = await _repository.UpdateItemAsync(userId, productId, request.Cantidad);
            if (!updated)
                throw new NotFoundException("CRT-002", $"El producto con ID {productId} no existe en el carrito.");

            _logger.LogInformation("Cantidad actualizada para producto {ProductoId} en carrito de usuario {UserId}",
                productId, userId);

            var items = await _repository.GetItemsAsync(userId);
            return MapToResponse(userId, items);
        }

        public async Task RemoveItemAsync(Guid userId, string productId)
        {
            var cart = await _repository.GetCartAsync(userId);
            if (cart == null)
                throw new NotFoundException("CRT-001", $"El usuario con ID {userId} no tiene un carrito activo.");

            var removed = await _repository.DeleteItemAsync(userId, productId);
            if (!removed)
                throw new NotFoundException("CRT-002", $"El producto con ID {productId} no existe en el carrito.");

            _logger.LogInformation("Producto {ProductoId} eliminado del carrito del usuario {UserId}",
                productId, userId);
        }

        public async Task ClearCartAsync(Guid userId)
        {
            var cart = await _repository.GetCartAsync(userId);
            if (cart == null)
                throw new NotFoundException("CRT-001", $"El usuario con ID {userId} no tiene un carrito activo.");

            await _repository.ClearCartAsync(userId);
            _logger.LogInformation("Carrito del usuario {UserId} vaciado", userId);
        }

        private CartResponse MapToResponse(Guid userId, IEnumerable<Models.CartItemEntity> items)
        {
            return new CartResponse
            {
                UsuarioId = userId,
                Items = items.Select(i => new CartItemDTO
                {
                    ProductoId = i.ProductoId,
                    Cantidad = i.Cantidad
                }).ToList(),
                FechaActualizacion = DateTime.UtcNow
            };
        }

        private void ValidarCantidad(int cantidad)
        {
            if (cantidad <= 0)
                throw new ValidationException("CRT-004", "La cantidad debe ser mayor a cero.");
        }

        private void ValidarProductoId(string productoId)
        {
            if (string.IsNullOrWhiteSpace(productoId))
                throw new NotFoundException("CRT-002", "El ID del producto no puede estar vacío.");
        }
    }
}