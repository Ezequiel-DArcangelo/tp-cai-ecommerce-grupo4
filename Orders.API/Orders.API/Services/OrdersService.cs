using Orders.API.Data;
using Orders.API.DTOs;
using Orders.API.Exceptions;
using Orders.API.Models;

namespace Orders.API.Services
{
    public class OrdersService
    {
        private readonly OrdersRepository _repository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OrdersService> _logger;

        public OrdersService(OrdersRepository repository, IHttpClientFactory httpClientFactory, ILogger<OrdersService> logger)
        {
            _repository = repository;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // Obtener todas las órdenes (con filtro opcional por usuarioId)
        public async Task<List<OrderResponse>> GetOrders(string? usuarioId = null)
        {
            var orders = await _repository.GetAllAsync(usuarioId);
            return orders.Select(o => MapToResponse(o)).ToList();
        }

        // Obtener una orden por ID
        public async Task<OrderResponse> GetOrderById(string id)
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
            bool usuarioExiste = await ExisteUsuario(request.UsuarioId);
            if (!usuarioExiste)
                throw new NotFoundException("ORD-003", $"El usuario con ID {request.UsuarioId} no existe.");

            // Validar productos en Products API (ORD-004 y ORD-005)
            foreach (var item in request.Items)
            {
                var producto = await GetProducto(item.ProductoId);
                if (producto == null)
                    throw new NotFoundException("ORD-004", $"El producto con ID {item.ProductoId} no existe.");

                if (producto.Stock < item.Cantidad)
                    throw new BusinessRuleException("ORD-005",
                        $"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock}, solicitado: {item.Cantidad}.");
            }

            // Crear la orden
            var newOrder = new Order
            {
                Id = Guid.NewGuid().ToString(),
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
        public async Task<OrderResponse> UpdateOrderStatus(string id, UpdateOrderStatusRequest request)
        {
            var order = await _repository.GetByIdAsync(id);
            if (order == null)
                throw new NotFoundException("ORD-001", $"La orden con ID {id} no existe.");

            // Validar transición de estado (ORD-006)
            if (order.Estado == "Entregada" && request.NuevoEstado == "Pendiente")
                throw new BusinessRuleException("ORD-006",
                    $"Con orden en estado 'Entregada' no puede volver a 'Pendiente'.");

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
                FechaCreacion = order.FechaCreacion
            };
        }

        // Validar datos de creación de orden
        private void ValidarCreateOrderRequest(CreateOrderRequest request)
        {
            List<string> errores = new List<string>();

            if (string.IsNullOrWhiteSpace(request.UsuarioId))
                errores.Add("El UsuarioId es obligatorio y debe ser válido.");

            if (request.Items == null || request.Items.Count == 0)
                errores.Add("Debe incluir al menos un producto en la orden.");

            if (errores.Count > 0)
                throw new ValidationException("ORD-002", string.Join(" ", errores));
        }

        // Validar usuario en Users API (ORD-003)
        private async Task<bool> ExisteUsuario(string usuarioId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var respuesta = await client.GetAsync($"https://localhost:7206/api/users/{usuarioId}");
                return respuesta.StatusCode != System.Net.HttpStatusCode.NotFound;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar Users API para usuario {UsuarioId}", usuarioId);
                throw new BusinessRuleException("ORD-007", "Error al comunicarse con Users API.");
            }
        }

        // Obtener producto de Products API (ORD-004, ORD-005)
        private async Task<ProductoDto?> GetProducto(string productoId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var respuesta = await client.GetAsync($"https://localhost:7196/api/products/{productoId}");
                if (respuesta.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                return await respuesta.Content.ReadFromJsonAsync<ProductoDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar Products API para producto {ProductoId}", productoId);
                throw new BusinessRuleException("ORD-007", "Error al comunicarse con Products API.");
            }
        }
    }

    public class ProductoDto
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int Stock { get; set; }
        public decimal Precio { get; set; }
    }
}