namespace Data.API.Options;

public class NlManagerInfoOptions
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Protocol { get; set; } = "http";
    public string Host { get; set; } = default!;
    public string HostSuffix { get; set; } = string.Empty;
    public int Port { get; set; } = 8080;
    public string Region { get; set; } = default!;
}
