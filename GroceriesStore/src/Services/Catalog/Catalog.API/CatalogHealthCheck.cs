using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Catalog.API
{
    public class CatalogHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            // All is well!
            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }
}
