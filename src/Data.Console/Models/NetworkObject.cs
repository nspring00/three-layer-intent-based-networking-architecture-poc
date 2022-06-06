namespace Data.Console.Models;

public class NetworkObject
{
    public Region Region { get; set; } = default!;
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime? LastUpdate { get; set; }
    public Dictionary<TimeSpan, NetworkObjectInfo> Infos { get; } = new();
}
