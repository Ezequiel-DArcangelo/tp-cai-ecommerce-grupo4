using Orders.API.Data;
using Orders.API.DTOs;
using Orders.API.Exceptions;
using Orders.API.Models;

namespace Orders.API.Services
{
    public class OrdersService
    {
        private readonly OrdersRepository _repository;
        private readonly UsersApiClient _usersApiClient;
        private readonly ProductsApiClient _productsApiClient;
        private readonly ILogger<OrdersService> _logger;

        public OrdersService(
            OrdersRepository repository,
            UsersApiClient usersApiClient,
            ProductsApiClient productsApiClient,
            ILogger<OrdersService> logger)
        {
            _repository = repository;
            _usersApiClient = usersApiClient;
            _productsApiClient = productsApiClient;
            _logger = logger;
        }

        // Obtener todas las órdenes (con filtro opcional por usuarioId)
        public async Task<List<OrderResponse>> GetOrders(Guid? usuarioId = null)
        {
            var orders = await _repository.GetAllAsync(usuarioId);
            return orders.Select(MapToResponse).ToList();
        }

        // Obtener una orden por ID
        public async Task<OrderResponse> GetOrderById(Guid id)
        {
            var order = await _repository.GetByIdAsync(id);
            if (order == null)
                throw new NotFoundException("ORD-001", $"La orden con ID {id} no existe.");

            return MapToResponse(order);
        }

        // Crear una nueva orden
        public async Task<OrderResponse> CreateOrder(CreateOrderRequest request)
        {
            // Validar campos (ORD-002)
            ValidarCreateOrderRequest(request);

            // Validar que el usuario exista en Users API (ORD-003)
            bool usuarioExiste = await _usersApiClient.ExisteUsuario(request.UsuarioId);
            if (!usuarioExiste)
                throw new NotFoundException("ORD-003", $"El usuario con ID {request.UsuarioId} no existe.");

            // Validar productos en Products API (ORD-004 y ORD-005)
            foreach (var item in request.Items)
            {
                var producto = await _productsApiClient.GetProducto(item.ProductoId);
                if (producto == null)
                    throw new NotFoundException("ORD-004", $"El producto con ID {item.ProductoId} no existe.");

                if (producto.Stock < item.Cantidad)
                    throw new BusinessRuleException("ORD-005",
                        $"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock}, solicitado: {item.Cantidad}.");

                // Capturar precio unitario desde Products API
                item.PrecioUnitario = producto.Precio;
            }

            // Crear la orden
            var newOrder = new Order
            {
                Id = Guid.NewGuid(),
                UsuarioId = request.UsuarioId,
                Items = request.Items,
                Total = request.Items.Sum(i => i.Cantidad * i.PrecioUnitario),
                Estado = "Pendiente",
                FechaCreacion = DateTime.UtcNow.ToString("o")
            };

            var created = await _repository.CreateAsync(newOrder);
            _logger.LogInformation("Orden {OrderId} creada para usuario {UsuarioId}", created.Id, created.UsuarioId);
            return MapToResponse(created);
        }

        // Actualizar estado de una orden
        public async Task<OrderResponse> UpdateOrderStatus(Guid id, UpdateOrderStatusRequest request)
        {
            var order = await _repository.GetByIdAsync(id);
            if (order == null)
                throw new NotFoundException("ORD-001", $"La orden con ID {id} no existe.");

            // Validar transición de estado (ORD-006)
            if (order.Estado == "Entregada" && request.NuevoEstado == "Pendiente")
                throw new BusinessRuleException("ORD-006",
                    $"Una orden en estado 'Entregada' no puede volver a 'Pendiente'.");

            await _repository.UpdateStatusAsync(id, request.NuevoEstado);
            _logger.LogInformation("Orden {OrderId} actualizada a estado {Estado}", id, request.NuevoEstado);

            var updated = await _repository.GetByIdAsync(id);
            return MapToResponse(updated!);
        }

        // Verificar si un producto tiene órdenes activas (para Products API)
        public async Task<bool> ExisteProductoEnOrdenesActivas(string productoId)
        {
            return await _repository.ExisteProductoEnOrdenesActivasAsync(productoId);
        }

        // Auxiliar: convertir Order en OrderResponse
        private OrderResponse MapToResponse(Order order)
        {
            return new OrderResponse
            {
                Id = order.Id,
                UsuarioId = order.UsuarioId,
                Items = order.Items,
                Total = order.Total,
                Estado = order.Estado,
                FechaCreacion = DateTime.UtcNow
            };
        }

        // Validar datos de creación de orden
        private void ValidarCreateOrderRequest(CreateOrderRequest request)
        {
            List<string> errores = new();

            if (request.UsuarioId == Guid.Empty)
                errores.Add("El UsuarioId es obligatorio y debe ser válido.");

            if (request.Items == null || request.Items.Count == 0)
                errores.Add("Debe incluir al menos un producto en la orden.");

            if (errores.Count > 0)
                throw new ValidationException("ORD-002", string.Join(" ", errores));
        }
    }
}
