using Microsoft.Extensions.AI;

namespace ItDepends.API.Features.SmartBoolean;

public interface ISmartBooleanService
{
    Task<string> GetRawResponseAsync(string userPrompt, CancellationToken cancellationToken);
}

public class SmartBooleanService : ISmartBooleanService
{
    private readonly IChatClient _client;
    private const string SystemPrompt = "You are a helpful assistant that always responds with either true or false based on the user's question. Answer only with true or false, without any additional text.";

    public SmartBooleanService(IChatClient client)
    {
        _client = client;
    }

    public async Task<string> GetRawResponseAsync(string userPrompt, CancellationToken cancellationToken)
    {
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, SystemPrompt),
            new(ChatRole.User, userPrompt)
        };

        var response = await _client.GetResponseAsync(messages, cancellationToken: cancellationToken);
        return response.Text.Trim();
    }
}
