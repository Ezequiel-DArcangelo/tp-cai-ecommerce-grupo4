using Dapper;
using Microsoft.Data.Sqlite;
using Cart.API.Models;

namespace Cart.API.Data
{
    public class CartRepository
    {
        private readonly IConfiguration _config;

        public CartRepository(IConfiguration config) => _config = config;

        private SqliteConnection CreateConnection() =>
            new(_config.GetConnectionString("DefaultConnection") ?? "Data Source=cart.db");

        // ── GET CART ──────────────────────────────────────────────────────────
        public async Task<CartEntity?> GetCartAsync(Guid usuarioId)
        {
            using var conn = CreateConnection();
            return await conn.QuerySingleOrDefaultAsync<CartEntity>("""
                SELECT id, usuario_id AS UsuarioId,
                       created_at AS CreatedAt, updated_at AS UpdatedAt
                FROM carts
                WHERE usuario_id = @UsuarioId
            """, new { UsuarioId = usuarioId.ToString() });
        }

        // ── GET ITEMS ─────────────────────────────────────────────────────────
        public async Task<IEnumerable<CartItemEntity>> GetItemsAsync(Guid usuarioId)
        {
            using var conn = CreateConnection();
            return await conn.QueryAsync<CartItemEntity>("""
                SELECT id, usuario_id AS UsuarioId,
                       producto_id AS ProductoId, cantidad AS Cantidad,
                       updated_at AS UpdatedAt
                FROM cart_items
                WHERE usuario_id = @UsuarioId
            """, new { UsuarioId = usuarioId.ToString() });
        }

        // ── CREATE CART ───────────────────────────────────────────────────────
        public async Task CreateCartAsync(Guid usuarioId)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync("""
                INSERT OR IGNORE INTO carts (usuario_id)
                VALUES (@UsuarioId);
            """, new { UsuarioId = usuarioId.ToString() });
        }

        // ── ADD OR UPDATE ITEM ────────────────────────────────────────────────
        public async Task AddOrUpdateItemAsync(Guid usuarioId, Guid productoId, int cantidad)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync("""
                INSERT INTO cart_items (usuario_id, producto_id, cantidad)
                VALUES (@UsuarioId, @ProductoId, @Cantidad)
                ON CONFLICT(usuario_id, producto_id)
                DO UPDATE SET cantidad  = cantidad + @Cantidad,
                              updated_at = datetime('now');
            """, new
            {
                UsuarioId = usuarioId.ToString(),
                ProductoId = productoId.ToString(),
                Cantidad = cantidad
            });

            await UpdateCartTimestampAsync(usuarioId, conn);
        }

        // ── UPDATE ITEM QUANTITY ──────────────────────────────────────────────
        public async Task<bool> UpdateItemAsync(Guid usuarioId, Guid productoId, int cantidad)
        {
            using var conn = CreateConnection();
            var rows = await conn.ExecuteAsync("""
                UPDATE cart_items
                SET cantidad   = @Cantidad,
                    updated_at = datetime('now')
                WHERE usuario_id  = @UsuarioId
                  AND producto_id = @ProductoId
            """, new
            {
                UsuarioId = usuarioId.ToString(),
                ProductoId = productoId.ToString(),
                Cantidad = cantidad
            });

            if (rows > 0) await UpdateCartTimestampAsync(usuarioId, conn);
            return rows > 0;
        }

        // ── DELETE ITEM ───────────────────────────────────────────────────────
        public async Task<bool> DeleteItemAsync(Guid usuarioId, Guid productoId)
        {
            using var conn = CreateConnection();
            var rows = await conn.ExecuteAsync("""
                DELETE FROM cart_items
                WHERE usuario_id  = @UsuarioId
                  AND producto_id = @ProductoId
            """, new
            {
                UsuarioId = usuarioId.ToString(),
                ProductoId = productoId.ToString()
            });

            if (rows > 0) await UpdateCartTimestampAsync(usuarioId, conn);
            return rows > 0;
        }

        // ── CLEAR CART ────────────────────────────────────────────────────────
        public async Task<bool> ClearCartAsync(Guid usuarioId)
        {
            using var conn = CreateConnection();
            var rows = await conn.ExecuteAsync("""
                DELETE FROM cart_items
                WHERE usuario_id = @UsuarioId
            """, new { UsuarioId = usuarioId.ToString() });

            if (rows > 0) await UpdateCartTimestampAsync(usuarioId, conn);
            return rows > 0;
        }

        // ── AUXILIAR ──────────────────────────────────────────────────────────
        private async Task UpdateCartTimestampAsync(Guid usuarioId, SqliteConnection conn)
        {
            await conn.ExecuteAsync("""
                UPDATE carts SET updated_at = datetime('now')
                WHERE usuario_id = @UsuarioId
            """, new { UsuarioId = usuarioId.ToString() });
        }
    }
}