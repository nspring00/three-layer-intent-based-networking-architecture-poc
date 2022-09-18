namespace Knowledge.API.Contracts.Responses;

public class GetIntentsResponse
{
    public List<GetIntentResponse> Intents { get; set; } = new();
}