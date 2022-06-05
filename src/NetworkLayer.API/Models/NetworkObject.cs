namespace NetworkLayer.API.Models;

public record NetworkObject
{
    public int Id { get; set; }
    public string Ip { get; set; } = default!;
    public List<string> Groups { get; set; } = new();
    public string Application { get; set; } = default!;
    public HardwareInfo Hardware { get; set; } = new();
    public Utilization Utilization { get; set; } = new();
    public float Availability { get; set; }
}
