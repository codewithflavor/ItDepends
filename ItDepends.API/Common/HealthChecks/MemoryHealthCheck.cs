using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ItDepends.API.Common.HealthChecks;

public class MemoryHealthCheck(ILogger<MemoryHealthCheck> logger) : IHealthCheck
{
    private const long WarningThresholdBytes = 1_500_000_000; // 1.5 GB
    private const long UnhealthyThresholdBytes = 2_000_000_000; // 2 GB

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var allocated = GC.GetTotalMemory(forceFullCollection: false);
        var data = new Dictionary<string, object>
        {
            { "allocatedBytes", allocated },
            { "allocatedMB", allocated / 1024 / 1024 },
            { "gen0Collections", GC.CollectionCount(0) },
            { "gen1Collections", GC.CollectionCount(1) },
            { "gen2Collections", GC.CollectionCount(2) }
        };

        switch (allocated)
        {
            case >= UnhealthyThresholdBytes:
                logger.LogWarning("Memory usage is critically high: {AllocatedMB} MB", allocated / 1024 / 1024);
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    $"Memory usage is critically high: {allocated / 1024 / 1024} MB",
                    data: data));
            case >= WarningThresholdBytes:
                logger.LogWarning("Memory usage is elevated: {AllocatedMB} MB", allocated / 1024 / 1024);
                return Task.FromResult(HealthCheckResult.Degraded(
                    $"Memory usage is elevated: {allocated / 1024 / 1024} MB",
                    data: data));
            default:
                return Task.FromResult(HealthCheckResult.Healthy(
                    $"Memory usage is normal: {allocated / 1024 / 1024} MB",
                    data));
        }
    }
}

