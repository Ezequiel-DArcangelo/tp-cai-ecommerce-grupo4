using Dapper;
using Microsoft.Data.Sqlite;

namespace Orders.API.Data
{
    public class OrdersDatabaseInitializer
    {
        private readonly IConfiguration _config;
        private readonly ILogger<OrdersDatabaseInitializer> _logger;

        public OrdersDatabaseInitializer(IConfiguration config, ILogger<OrdersDatabaseInitializer> logger)
        {
            _config = config;
            _logger = logger;
        }

        public void Initialize()
        {
            var connectionString = _config.GetConnectionString("DefaultConnection")
                ?? "Data Source=orders.db";

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            connection.Execute("""
                CREATE TABLE IF NOT EXISTS orders (
                    id          TEXT    PRIMARY KEY,
                    usuario_id  TEXT    NOT NULL,
                    total       REAL    NOT NULL DEFAULT 0,
                    estado      TEXT    NOT NULL DEFAULT 'Pendiente',
                    created_at  TEXT    NOT NULL DEFAULT (datetime('now'))
                );
            """);

            connection.Execute("""
                CREATE TABLE IF NOT EXISTS order_items (
                    id                INTEGER PRIMARY KEY AUTOINCREMENT,
                    order_id          TEXT    NOT NULL,
                    producto_id       TEXT    NOT NULL,
                    cantidad          INTEGER NOT NULL DEFAULT 1,
                    precio_unitario   REAL    NOT NULL DEFAULT 0
                );
            """);

            _logger.LogInformation("SQLite inicializado correctamente → {db}", connectionString);
        }
    }
}