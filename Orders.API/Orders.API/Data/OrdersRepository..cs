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

        // ── GET ALL ───────────────────────────────────────────────────────────
        public async Task<IEnumerable<Order>> GetAllAsync(string? usuarioId = null)
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

        // ── GET BY ID ─────────────────────────────────────────────────────────
        public async Task<Order?> GetByIdAsync(string id)
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

        // ── GET ITEMS ─────────────────────────────────────────────────────────
        public async Task<IEnumerable<OrderItemDTO>> GetItemsAsync(string orderId)
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

        // ── CREATE ────────────────────────────────────────────────────────────
        public async Task<Order> CreateAsync(Order order)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync("""
                INSERT INTO orders (id, usuario_id, total, estado)
                VALUES (@Id, @UsuarioId, @Total, @Estado)
            """, new
            {
                Id = order.Id,
                UsuarioId = order.UsuarioId,
                Total = order.Total,
                Estado = order.Estado
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

        // ── UPDATE STATUS ─────────────────────────────────────────────────────
        public async Task<bool> UpdateStatusAsync(string id, string nuevoEstado)
        {
            using var conn = CreateConnection();
            var rows = await conn.ExecuteAsync("""
                UPDATE orders SET estado = @Estado
                WHERE id = @Id
            """, new { Id = id, Estado = nuevoEstado });

            return rows > 0;
        }

        // ── CHECK PRODUCTO EN ORDENES ACTIVAS ─────────────────────────────────
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