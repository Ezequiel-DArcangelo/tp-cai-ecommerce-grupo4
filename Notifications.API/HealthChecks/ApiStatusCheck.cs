using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Notifications.API.HealthChecks
{
    public class ApiStatusCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HealthCheckResult.Healthy($"API operativa. .NET Version: {Environment.Version}"));
        }
    }
}
