namespace ItDepends.API.Common.HealthChecks;

/// <summary>
/// Represents the health check response
/// </summary>
public class HealthCheckResponse
{
    /// <summary>
    /// Overall health status (Healthy, Degraded, or Unhealthy)
    /// </summary>
    public required string Status { get; init; }
    
    /// <summary>
    /// Timestamp when the health check was performed
    /// </summary>
    public required DateTime Timestamp { get; init; }
    
    /// <summary>
    /// Total duration of all health checks
    /// </summary>
    public required TimeSpan Duration { get; init; }
    
    /// <summary>
    /// Individual health check results
    /// </summary>
    public required IEnumerable<HealthCheckEntry> Checks { get; init; }
}

/// <summary>
/// Represents an individual health check entry
/// </summary>
public class HealthCheckEntry
{
    /// <summary>
    /// Name of the health check
    /// </summary>
    public required string Name { get; init; }
    
    /// <summary>
    /// Status of this specific check (Healthy, Degraded, or Unhealthy)
    /// </summary>
    public required string Status { get; init; }
    
    /// <summary>
    /// Description or message from the health check
    /// </summary>
    public string? Description { get; init; }
    
    /// <summary>
    /// Duration of this specific health check
    /// </summary>
    public required TimeSpan Duration { get; init; }
    
    /// <summary>
    /// Exception message if the health check failed
    /// </summary>
    public string? Exception { get; init; }
    
    /// <summary>
    /// Additional diagnostic data from the health check
    /// </summary>
    public IReadOnlyDictionary<string, object>? Data { get; init; }
}

