# Health Check Endpoints

This document describes the health check endpoints available in the ItDepends.API project.

## Available Endpoints

### 1. `/health` - Comprehensive Health Check
Returns the status of all registered health checks.

**Example Response:**
```json
{
  "status": "Healthy",
  "timestamp": "2026-01-04T10:30:45.1234567Z",
  "duration": "00:00:01.2345678",
  "checks": [
    {
      "name": "openai",
      "status": "Healthy",
      "description": "OpenAI service is responding normally.",
      "duration": "00:00:01.1234567",
      "exception": null,
      "data": {}
    },
    {
      "name": "memory",
      "status": "Healthy",
      "description": "Memory usage is normal: 125 MB",
      "duration": "00:00:00.0012345",
      "exception": null,
      "data": {
        "allocatedBytes": 131072000,
        "allocatedMB": 125,
        "gen0Collections": 5,
        "gen1Collections": 2,
        "gen2Collections": 1
      }
    }
  ]
}
```

### 2. `/health/live` - Liveness Probe
Checks if the application is running and responsive. This is ideal for Kubernetes liveness probes.

Returns only health checks tagged with `"live"`:
- **memory** - Memory usage check

**Use Case:** Determines if the application should be restarted.

### 3. `/health/ready` - Readiness Probe
Checks if the application is ready to serve traffic. This is ideal for Kubernetes readiness probes.

Returns only health checks tagged with `"ready"`:
- **openai** - OpenAI service connectivity check

**Use Case:** Determines if the application should receive traffic.

## Health Check Implementations

### OpenAI Health Check
- **Name:** `openai`
- **Tags:** `ready`, `external`
- **Purpose:** Verifies connectivity to the OpenAI service
- **Timeout:** 10 seconds
- **Test:** Sends a simple test message and validates response

**Status Meanings:**
- **Healthy:** OpenAI is responding normally
- **Degraded:** OpenAI timed out or returned empty response
- **Unhealthy:** OpenAI service is not accessible or returned an error

### Memory Health Check
- **Name:** `memory`
- **Tags:** `live`, `memory`
- **Purpose:** Monitors application memory usage
- **Thresholds:**
  - **Warning (Degraded):** > 1.5 GB
  - **Critical (Unhealthy):** > 2 GB

**Additional Data:**
- Total allocated bytes
- Allocated MB
- GC collection counts (Gen 0, Gen 1, Gen 2)

## HTTP Status Codes

The health check endpoints return different HTTP status codes based on the overall health:

- **200 OK:** All checks are Healthy
- **200 OK:** One or more checks are Degraded (still operational but with warnings)
- **503 Service Unavailable:** One or more checks are Unhealthy

## Integration with Monitoring

### Kubernetes Example

```yaml
apiVersion: v1
kind: Pod
metadata:
  name: itdepends-api
spec:
  containers:
  - name: api
    image: itdepends-api:latest
    livenessProbe:
      httpGet:
        path: /health/live
        port: 8080
      initialDelaySeconds: 30
      periodSeconds: 10
      timeoutSeconds: 5
      failureThreshold: 3
    readinessProbe:
      httpGet:
        path: /health/ready
        port: 8080
      initialDelaySeconds: 10
      periodSeconds: 5
      timeoutSeconds: 10
      failureThreshold: 3
```

### Docker Compose Example

```yaml
services:
  api:
    image: itdepends-api:latest
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s
```

### Testing with cURL

```bash
# Check all health checks
curl http://localhost:5244/health

# Check liveness only
curl http://localhost:5244/health/live

# Check readiness only
curl http://localhost:5244/health/ready

# Pretty print JSON response
curl http://localhost:5244/health | jq
```

## Adding Custom Health Checks

To add a new health check:

1. Create a new class implementing `IHealthCheck`:

```csharp
public class CustomHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        // Your health check logic here
        var isHealthy = await CheckSomethingAsync();
        
        return isHealthy 
            ? HealthCheckResult.Healthy("Service is healthy")
            : HealthCheckResult.Unhealthy("Service is unhealthy");
    }
}
```

2. Register it in `Program.cs`:

```csharp
builder.Services.AddHealthChecks()
    .AddCheck<CustomHealthCheck>("custom", tags: new[] { "ready" });
```

## Best Practices

1. **Separate Liveness and Readiness:** Use `/health/live` for restart decisions and `/health/ready` for traffic routing
2. **Set Appropriate Timeouts:** Ensure health checks complete quickly (< 10 seconds)
3. **Monitor Health Check Failures:** Alert on repeated failures
4. **Include Relevant Data:** Add diagnostic data to help troubleshoot issues
5. **Don't Check Everything in Liveness:** Keep liveness checks simple to avoid false restarts

