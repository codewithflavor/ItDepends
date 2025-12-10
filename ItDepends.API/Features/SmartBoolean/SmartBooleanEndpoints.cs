using Microsoft.AspNetCore.Mvc;
using ItDepends.API.Common;
using Microsoft.Extensions.AI;

namespace ItDepends.API.Features.SmartBoolean;

public static class SmartBooleanEndpoints
{
    public static IEndpointRouteBuilder MapSmartBooleanEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/api/smart")
            .MapPost("/boolean", async ([FromBody] SmartBooleanRequest request, [FromServices] ISmartBooleanService service, [FromServices] ILogger<Program> logger, CancellationToken cancellationToken) =>
            {
                logger.LogInformation("SmartBoolean request received. PromptLength={PromptLength}", request.Prompt?.Length ?? 0);

                try
                {
                    var resultText = (await service.GetRawResponseAsync(request.Prompt!, cancellationToken)).ToLowerInvariant();

                    logger.LogInformation("SmartBoolean model returned '{ResultText}'", resultText);

                    if (bool.TryParse(resultText, out var boolean))
                    {
                        var res = new SmartBooleanResponse
                        {
                            Value = boolean,
                            Raw = resultText,
                        };

                        logger.LogInformation("SmartBoolean parsed successfully: {Value}", boolean);
                        return Results.Ok(res);
                    }

                    logger.LogWarning("SmartBoolean model response not parsable as boolean: '{ResultText}'", resultText);
                    return Results.Problem(
                        title: "Invalid model response",
                        detail: $"Model response '{resultText}' could not be parsed as a boolean.",
                        statusCode: 422
                    );
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "SmartBoolean external service error");
                    return Results.Problem(title: "External service error", detail: ex.Message, statusCode: 502);
                }
            })
            .AddEndpointFilter<ValidationFilter<SmartBooleanRequest>>()
            .WithName("GetSmartBoolean")
            .WithTags("Smart Boolean");

        return app;
    }
}