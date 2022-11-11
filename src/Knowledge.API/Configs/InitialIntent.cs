namespace Knowledge.API.Configs;

public class InitialIntent
{
    public string Kpi { get; set; } = default!;
    public string Region { get; set; } = default!;
    public string TargetMode { get; set; } = default!;
    public float TargetValue { get; set; }
}
