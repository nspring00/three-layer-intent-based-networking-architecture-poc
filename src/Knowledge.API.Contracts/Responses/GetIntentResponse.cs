namespace Knowledge.API.Contracts.Responses;

public class GetIntentResponse
{
    public string Region { get; set; } = default!;
    public string Kpi { get; set; } = default!;
    public string TargetMode { get; set; } = default!;
    public float Value { get; set; }
}