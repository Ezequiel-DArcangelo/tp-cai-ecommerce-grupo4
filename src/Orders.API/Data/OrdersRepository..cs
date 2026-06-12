using Dapper;
using Microsoft.Data.Sqlite;
using Orders.API.DTOs;
using Orders.API.Models;

namespace Orders.API.Data
{
    public class OrdersRepository
    {
        private readonly IConfiguration _config;

        public OrdersRepository(IConfiguration config) => _config = config;

        private SqliteConnection CreateConnection() =>
            new(_config.GetConnectionString("DefaultConnection") ?? "Data Source=orders.db");

        /// <summary>
        /// Obtiene todas las órdenes, con filtro opcional por usuario.
        /// </summary>
        /// <param name="usuarioId">Identificador del usuario (Guid) o null para traer todas.</param>
        /// <returns>Lista de órdenes con sus ítems.</returns>
        public async Task<IEnumerable<Order>> GetAllAsync(Guid? usuarioId = null)
        {
            using var conn = CreateConnection();
            var orders = await conn.QueryAsync<Order>("""
                SELECT id          AS Id,
                       usuario_id  AS UsuarioId,
                       total       AS Total,
                       estado      AS Estado,
                       created_at  AS FechaCreacion
                FROM orders
                WHERE (@UsuarioId IS NULL OR usuario_id = @UsuarioId)
                ORDER BY created_at DESC
            """, new { UsuarioId = usuarioId });

            foreach (var order in orders)
                order.Items = (await GetItemsAsync(order.Id)).ToList();

            return orders;
        }

        /// <summary>
        /// Obtiene una orden por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la orden (Guid).</param>
        /// <returns>Orden encontrada o null si no existe.</returns>
        public async Task<Order?> GetByIdAsync(Guid id)
        {
            using var conn = CreateConnection();
            var order = await conn.QuerySingleOrDefaultAsync<Order>("""
                SELECT id          AS Id,
                       usuario_id  AS UsuarioId,
                       total       AS Total,
                       estado      AS Estado,
                       created_at  AS FechaCreacion
                FROM orders
                WHERE id = @Id
            """, new { Id = id });

            if (order != null)
                order.Items = (await GetItemsAsync(order.Id)).ToList();

            return order;
        }

        /// <summary>
        /// Obtiene los ítems de una orden.
        /// </summary>
        /// <param name="orderId">Identificador de la orden (Guid).</param>
        /// <returns>Lista de ítems de la orden.</returns>
        public async Task<IEnumerable<OrderItemDTO>> GetItemsAsync(Guid orderId)
        {
            using var conn = CreateConnection();
            return await conn.QueryAsync<OrderItemDTO>("""
                SELECT producto_id     AS ProductoId,
                       cantidad        AS Cantidad,
                       precio_unitario AS PrecioUnitario
                FROM order_items
                WHERE order_id = @OrderId
            """, new { OrderId = orderId });
        }

        /// <summary>
        /// Crea una nueva orden y sus ítems.
        /// </summary>
        /// <param name="order">Orden a crear.</param>
        /// <returns>Orden creada con sus ítems.</returns>
        public async Task<Order> CreateAsync(Order order)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync("""
                INSERT INTO orders (id, usuario_id, total, estado, created_at)
                VALUES (@Id, @UsuarioId, @Total, @Estado, @FechaCreacion)
            """, new
            {
                Id = order.Id,
                UsuarioId = order.UsuarioId,
                Total = order.Total,
                Estado = order.Estado,
                FechaCreacion = order.FechaCreacion
            });

            foreach (var item in order.Items)
            {
                await conn.ExecuteAsync("""
                    INSERT INTO order_items (order_id, producto_id, cantidad, precio_unitario)
                    VALUES (@OrderId, @ProductoId, @Cantidad, @PrecioUnitario)
                """, new
                {
                    OrderId = order.Id,
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.PrecioUnitario
                });
            }

            return (await GetByIdAsync(order.Id))!;
        }

        /// <summary>
        /// Actualiza el estado de una orden.
        /// </summary>
        /// <param name="id">Identificador de la orden (Guid).</param>
        /// <param name="nuevoEstado">Nuevo estado a asignar.</param>
        /// <returns>True si se actualizó, false si no existe.</returns>
        public async Task<bool> UpdateStatusAsync(Guid id, string nuevoEstado)
        {
            using var conn = CreateConnection();
            var rows = await conn.ExecuteAsync("""
                UPDATE orders SET estado = @Estado
                WHERE id = @Id
            """, new { Id = id, Estado = nuevoEstado });

            return rows > 0;
        }

        /// <summary>
        /// Verifica si un producto está presente en órdenes activas.
        /// </summary>
        /// <param name="productoId">Identificador del producto.</param>
        /// <returns>True si el producto está en órdenes activas.</returns>
        public async Task<bool> ExisteProductoEnOrdenesActivasAsync(string productoId)
        {
            using var conn = CreateConnection();
            var count = await conn.ExecuteScalarAsync<int>("""
                SELECT COUNT(*) FROM order_items oi
                INNER JOIN orders o ON o.id = oi.order_id
                WHERE oi.producto_id = @ProductoId
                  AND o.estado IN ('Pendiente', 'Confirmada')
            """, new { ProductoId = productoId });

            return count > 0;
        }
    }
}
