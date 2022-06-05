namespace Knowledge.API.Dtos;

public record ReasoningRequest
{
    public List<string> Regions { get; init; } = new();
}
