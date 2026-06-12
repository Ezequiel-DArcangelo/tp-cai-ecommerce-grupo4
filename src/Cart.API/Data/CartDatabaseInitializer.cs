using Dapper;
using Microsoft.Data.Sqlite;

namespace Cart.API.Data
{
    public class CartDatabaseInitializer
    {
        private readonly IConfiguration _config;
        private readonly ILogger<CartDatabaseInitializer> _logger;

        public CartDatabaseInitializer(IConfiguration config, ILogger<CartDatabaseInitializer> logger)
        {
            _config = config;
            _logger = logger;
        }

        public void Initialize()
        {
            var connectionString = _config.GetConnectionString("DefaultConnection")
                ?? "Data Source=cart.db";

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            connection.Execute("""
                CREATE TABLE IF NOT EXISTS carts (
                    id          INTEGER PRIMARY KEY AUTOINCREMENT,
                    usuario_id  TEXT    NOT NULL UNIQUE,
                    created_at  TEXT    NOT NULL DEFAULT (datetime('now')),
                    updated_at  TEXT
                );
            """);

            connection.Execute("""
                CREATE TABLE IF NOT EXISTS cart_items (
                    id          INTEGER PRIMARY KEY AUTOINCREMENT,
                    usuario_id  TEXT    NOT NULL,
                    producto_id TEXT    NOT NULL,
                    cantidad    INTEGER NOT NULL DEFAULT 1,
                    updated_at  TEXT,
                    UNIQUE(usuario_id, producto_id)
                );
            """);

            _logger.LogInformation("SQLite inicializado correctamente → {db}", connectionString);
        }
    }
}