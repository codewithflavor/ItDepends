using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ItDepends.API.Common.HealthChecks;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddApiHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<OpenAIHealthCheck>("openai", tags: ["ready", "external"])
            .AddCheck<MemoryHealthCheck>("memory", tags: ["live", "memory"]);

        return services;
    }
    
    public static IEndpointRouteBuilder MapApiHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        var healthCheckGroup = endpoints.MapGroup("/health")
            .WithTags("Health Checks")
            .WithOpenApi();

        healthCheckGroup.MapGet("", async (HealthCheckService healthCheckService) =>
        {
            var report = await healthCheckService.CheckHealthAsync();
            return Results.Ok(HealthCheckResponseWriter.FormatResponse(report));
        })
        .WithName("GetHealthCheck")
        .WithSummary("Get comprehensive health status")
        .WithDescription("Returns the status of all registered health checks including OpenAI service and memory usage.")
        .Produces<HealthCheckResponse>()
        .Produces(StatusCodes.Status503ServiceUnavailable);

        healthCheckGroup.MapGet("/live", async (HealthCheckService healthCheckService) =>
        {
            var report = await healthCheckService.CheckHealthAsync(check => check.Tags.Contains("live"));
            return Results.Ok(HealthCheckResponseWriter.FormatResponse(report));
        })
        .WithName("GetLivenessCheck")
        .WithSummary("Get liveness probe status")
        .WithDescription("Returns health checks tagged with 'live'. Used to determine if the application should be restarted.")
        .Produces<HealthCheckResponse>()
        .Produces(StatusCodes.Status503ServiceUnavailable);

        healthCheckGroup.MapGet("/ready", async (HealthCheckService healthCheckService) =>
        {
            var report = await healthCheckService.CheckHealthAsync(check => check.Tags.Contains("ready"));
            return Results.Ok(HealthCheckResponseWriter.FormatResponse(report));
        })
        .WithName("GetReadinessCheck")
        .WithSummary("Get readiness probe status")
        .WithDescription("Returns health checks tagged with 'ready'. Used to determine if the application should receive traffic.")
        .Produces<HealthCheckResponse>()
        .Produces(StatusCodes.Status503ServiceUnavailable);

        return endpoints;
    }
}

