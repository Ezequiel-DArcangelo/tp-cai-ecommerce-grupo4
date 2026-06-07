using Microsoft.Data.Sqlite;
using Dapper;

namespace Products.API
{
    public class DatabaseInitializer
    {
        private readonly IConfiguration _config;

        public DatabaseInitializer(IConfiguration config)
        {
            _config = config;
        }

        public void Initialize()
        {
            var connectionString = _config.GetConnectionString("DefaultConnection")?? "Data Source=app.db";

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Products 
            (Id TEXT PRIMARY KEY,
            Name TEXT NOT NULL,
            Description TEXT,
            Price REAL NOT NULL DEFAULT 0,
            Stock INTEGER NOT NULL DEFAULT 0,
            Category TEXT,
            CreatedAt TEXT NOT NULL
            );");
        }
    }
}
