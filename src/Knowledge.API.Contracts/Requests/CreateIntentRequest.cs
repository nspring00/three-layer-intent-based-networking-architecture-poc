namespace Knowledge.API.Contracts.Requests;

public class CreateIntentRequest
{
    public string Region { get; set; } = default!;
    public string Kpi { get; set; } = default!;
    public string TargetMode { get; set; } = default!;
    public float Value { get; set; } = default;
}
