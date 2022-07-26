namespace Knowledge.API.Contracts.Requests;

public record ReasoningRequest
{
    public List<string> Regions { get; init; } = new();
}
