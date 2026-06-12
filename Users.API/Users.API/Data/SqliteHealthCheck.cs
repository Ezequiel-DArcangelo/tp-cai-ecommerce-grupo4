using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Users.API.Data
{
    // Health check personalizado que verifica que la base SQLite responde.
    public class SqliteHealthCheck : IHealthCheck
    {
        private readonly string _connectionString;

        public SqliteHealthCheck(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("UsersDb");
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Abrir conexion y ejecutar una query trivial para verificar que la base responde
                SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();

                SqliteCommand command = new SqliteCommand("SELECT 1", connection);
                command.ExecuteScalar();

                connection.Close();

                // Si todo salio bien, la base esta sana
                return Task.FromResult(HealthCheckResult.Healthy("La base de datos SQLite responde correctamente."));
            }
            catch (Exception ex)
            {
                // Si algo fallo, la base no esta sana
                return Task.FromResult(HealthCheckResult.Unhealthy("La base de datos SQLite no responde.", ex));
            }
        }
    }
}