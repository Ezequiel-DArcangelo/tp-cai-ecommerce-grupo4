using Orders.API.DTOs;
using Orders.API.Models;
using Orders.API.Exceptions;

namespace Orders.API.Services
{
    public class OrdersService
    {
        // Lista en memoria que simula la base de datos
        private static List<Order> _orders = new List<Order>();
        private static int _nextId = 1;

        // Obtener todas las órdenes
        public List<OrderResponse> GetOrders()
        {
            return _orders.Select(o => MapToResponse(o)).ToList();
        }

        // Obtener una orden por ID
        public OrderResponse GetOrderById(int id)
        {
            Order order = _orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                throw new NotFoundException("ORD-001", $"La orden con ID {id} no existe.");
            }
            return MapToResponse(order);
        }

        // Crear una nueva orden
        public OrderResponse CreateOrder(CreateOrderRequest request)
        {
            // Validar campos (ORD-002)
            ValidarCreateOrderRequest(request);

            // Simular validación de usuario y productos (ORD-003)
            if (request.UsuarioId <= 0)
            {
                throw new NotFoundException("ORD-003", $"El usuario con ID {request.UsuarioId} no existe.");
            }

            foreach (var item in request.Items)
            {
                if (item.ProductoId <= 0)
                {
                    throw new NotFoundException("ORD-003", $"El producto con ID {item.ProductoId} no existe.");
                }

                // Simular stock insuficiente (ORD-005)
                if (item.Cantidad > 5) // ejemplo: límite ficticio
                {
                    throw new BusinessRuleException(
                        "ORD-005",
                        $"Stock insuficiente para 'Producto {item.ProductoId}'. Disponible: 5, solicitado: {item.Cantidad}."
                    );
                }
            }

            // Crear la orden
            Order newOrder = new Order
            {
                Id = _nextId++,
                UsuarioId = request.UsuarioId,
                Items = request.Items,
                Total = request.Items.Sum(i => i.Cantidad * 100), // precio ficticio
                Estado = "Pendiente",
                FechaCreacion = DateTime.UtcNow
            };

            _orders.Add(newOrder);
            return MapToResponse(newOrder);
        }

        // Actualizar estado de una orden
        public OrderResponse UpdateOrderStatus(int id, UpdateOrderStatusRequest request)
        {
            Order order = _orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                throw new NotFoundException("ORD-001", $"La orden con ID {id} no existe.");
            }

            // Validar transición de estado (ORD-006)
            if (order.Estado == "Entregada" && request.NuevoEstado == "Pendiente")
            {
                throw new BusinessRuleException(
                    "ORD-006",
                    $"Con orden en estado 'Entregada' no puede volver a 'Pendiente'."
                );
            }

            order.Estado = request.NuevoEstado;
            return MapToResponse(order);
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

            if (request.UsuarioId <= 0)
                errores.Add("El UsuarioId es obligatorio y debe ser válido.");

            if (request.Items == null || request.Items.Count == 0)
                errores.Add("Debe incluir al menos un producto en la orden.");

            if (errores.Count > 0)
            {
                string mensaje = string.Join(" ", errores);
                throw new ValidationException("ORD-002", mensaje);
            }
        }
    }
}