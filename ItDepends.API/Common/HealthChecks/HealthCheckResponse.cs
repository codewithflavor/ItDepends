namespace ItDepends.API.Common.HealthChecks;

public class HealthCheckResponse
{
    public required string Status { get; init; }
    
    public required DateTime Timestamp { get; init; }
    
    public required TimeSpan Duration { get; init; }
    
    public required IEnumerable<HealthCheckEntry> Checks { get; init; }
}

public class HealthCheckEntry
{
    public required string Name { get; init; }
    
    public required string Status { get; init; }
    
    public string? Description { get; init; }
    
    public required TimeSpan Duration { get; init; }
    
    public string? Exception { get; init; }
    
    public IReadOnlyDictionary<string, object>? Data { get; init; }
}