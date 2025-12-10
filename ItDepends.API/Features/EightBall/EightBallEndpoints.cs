using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ItDepends.API.Features.EightBall;

public static class EightBallEndpoints
{
    private static readonly string[] AvailableResponses =
    [
        "It is certain.", "It is decidedly so.", "Without a doubt.", "Yes - definitely.", "You may rely on it.",
        "As I see it, yes.", "Most likely.", "Outlook good.", "Yes.", "Signs point to yes.",
        "Reply hazy, try again.", "Ask again later.", "Better not tell you now.", "Cannot predict now.",
        "Concentrate and ask again.", "Don't count on it.", "My reply is no.", "My sources say no.",
        "Outlook not so good.", "Very doubtful."
    ];

    public static IEndpointRouteBuilder MapEightBallEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/api")
            .MapGet("/8ball", ([FromServices] ILogger<Program> logger) =>
            {
                try
                {
                    var resultIndex = RandomNumberGenerator.GetInt32(AvailableResponses.Length);

                    logger.LogInformation("8-ball selected index {Index} with response '{Response}'", resultIndex, AvailableResponses[resultIndex]);

                    return Results.Ok(new EightBallResponse
                    {
                        Value = AvailableResponses[resultIndex],
                        Raw = resultIndex.ToString()
                    });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed generating 8-ball response");
                    return Results.Problem(
                        title: "Failed to generate 8-ball response",
                        detail: ex.Message,
                        statusCode: 500
                    );
                }
            })
            .WithName("Get8BallResponse")
            .WithTags("8-Ball");

        return app;
    }
}