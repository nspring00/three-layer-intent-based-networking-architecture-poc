namespace Data.Console.Models;

public class NetworkObject
{
    public Region Region { get; set; } = default!;
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public string Application { get; set; } = default!;
    public Dictionary<DateTime, NetworkObjectInfo> Infos { get; } = new();
}
