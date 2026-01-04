namespace ItDepends.API.Features.Boolean;

public sealed class BooleanResponse
{
    public bool Value { get; init; }
    public string Raw { get; init; } = string.Empty;
}