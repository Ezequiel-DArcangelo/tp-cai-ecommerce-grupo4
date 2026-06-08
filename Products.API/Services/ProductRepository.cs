using Microsoft.Data.Sqlite;
using Dapper;
using Products.API_.Models;


namespace Products.API.Services
{
    public class ProductRepository
    {
        private readonly IConfiguration _config;
        public ProductRepository(IConfiguration config)
        {
            _config = config;
        }
        private SqliteConnection CreateConnection()
        {
            var connectionString = _config.GetConnectionString("DefaultConnection") ?? "Data Source=app.db";
            return new SqliteConnection(connectionString);
        }
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            using var conn = CreateConnection();
  
            return await conn.QueryAsync<Product>(@"SELECT Id, Name AS Nombre, Description AS Descripcion, 
            Price AS Precio, Stock, Category AS Categoria, CreatedAt FROM Products ORDER BY CreatedAt DESC");
        }

        public async Task CreateAsync(Product product)
        {
            using var conn = CreateConnection();
  
            await conn.ExecuteAsync(@"INSERT INTO Products (Id, Name, Description, Price, Stock, Category, CreatedAt)
            VALUES (@Id, @Nombre, @Descripcion, @Precio, @Stock, @Categoria, @CreatedAt)", product);
        }

        public async Task UpdateAsync(Product product)
        {
            using var conn = CreateConnection();

            // Enviamos el script SQL para impactar los cambios en el archivo de la base de datos
            await conn.ExecuteAsync(@"UPDATE Products SET Name = @Nombre, Description = @Descripcion, 
            Price = @Precio, Stock = @Stock, Category = @Categoria WHERE Id = @Id", product);
        }

        public async Task DeleteAsync(string id)
        {
            using var conn = CreateConnection();

            // Ejecutamos el comando SQL para borrar el registro físico de la tabla
            await conn.ExecuteAsync("DELETE FROM Products WHERE Id = @Id", new { Id = id });
        }
    }
}   
