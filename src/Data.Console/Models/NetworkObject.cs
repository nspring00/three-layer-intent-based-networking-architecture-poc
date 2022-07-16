using Common.Models;

namespace Data.Console.Models;

public class NetworkObject
{
    public int NetworkLayerId { get; set; }
    public Region Region { get; set; } = default!;  // TODO 
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public string Application { get; set; } = default!;
    public Dictionary<DateTime, NetworkObjectInfo> Infos { get; } = new();
}
