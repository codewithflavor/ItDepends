using Microsoft.Extensions.AI;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ItDepends.API.Common.HealthChecks;

public class OpenAIHealthCheck(IChatClient chatClient, ILogger<OpenAIHealthCheck> logger) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, "You are a test assistant. Respond with 'OK'."),
                new(ChatRole.User, "Health check")
            };

            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            var response = await chatClient.GetResponseAsync(messages, cancellationToken: linkedCts.Token);

            return !string.IsNullOrEmpty(response.Text)
                ? HealthCheckResult.Healthy("OpenAI service is responding normally.")
                : HealthCheckResult.Degraded("OpenAI service returned empty response.");
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("OpenAI health check timed out");
            return HealthCheckResult.Degraded("OpenAI service health check timed out.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "OpenAI health check failed");
            return HealthCheckResult.Unhealthy(
                "OpenAI service is not responding.",
                ex,
                new Dictionary<string, object>
                {
                    { "error", ex.Message },
                    { "errorType", ex.GetType().Name }
                });
        }
    }
}

