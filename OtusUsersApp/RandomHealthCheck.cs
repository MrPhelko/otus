using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace OtusUsersApp
{
    public class RandomHealthCheck : IHealthCheck
    {
        private static readonly Random _rnd = new Random();

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }
}
