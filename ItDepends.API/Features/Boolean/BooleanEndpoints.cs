using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ItDepends.API.Features.Boolean;

public static class BooleanEndpoints
{
    public static IEndpointRouteBuilder MapBooleanEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/api")
            .MapGet("/boolean/random", ([FromServices] ILogger<Program> logger) =>
            {
                try
                {
                    var result = RandomNumberGenerator.GetInt32(0, 2);

                    logger.LogInformation("Generated random boolean {@Result}", result);

                    return Results.Ok(new BooleanResponse
                    {
                        Value = result == 0,
                        Raw = result.ToString()
                    });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to generate random boolean");
                    return Results.Problem(
                        title: "Failed to generate random boolean",
                        detail: ex.Message,
                        statusCode: 500
                    );
                }
            }).WithName("GetRandomBoolean")
            .WithTags("Boolean");

        app.MapGroup("/api")
            .MapGet("/boolean/false", ([FromServices] ILogger<Program> logger) =>
            {
                logger.LogInformation("Returning static boolean value false");
                return Results.Ok(new BooleanResponse
                {
                    Value = false,
                    Raw = "false"
                });
            })
            .WithName("GetFalseBoolean")
            .WithTags("Boolean");

        app.MapGroup("/api")
            .MapGet("/boolean/true", ([FromServices] ILogger<Program> logger) =>
            {
                logger.LogInformation("Returning static boolean value true");
                return Results.Ok(new BooleanResponse
                {
                    Value = true,
                    Raw = "true"
                });
            })
            .WithName("GetTrueBoolean")
            .WithTags("Boolean");

        return app;
    }
}