using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Products.API.Services
{
    public class SqliteHealthCheck : IHealthCheck
    {
        private readonly IConfiguration _config; 
        public SqliteHealthCheck(IConfiguration config)
        {
            _config = config;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            try
            {
              var connectionString = _config.GetConnectionString("DefaultConnection")?? "Data Source=app.db";
                using var connection = new SqliteConnection(connectionString);
                await connection.OpenAsync(cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT 1;";
                await command.ExecuteScalarAsync(cancellationToken);

                return HealthCheckResult.Healthy("La base de datos SQLite esta respondiendo correctamente.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("La base de datos SQLite no esta disponible.", ex);
            }
        }
    
    }
}
