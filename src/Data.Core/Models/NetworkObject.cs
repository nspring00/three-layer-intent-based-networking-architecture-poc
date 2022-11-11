using Common.Models;

namespace Data.Core.Models;

public class NetworkObject
{
    public int NetworkLayerId { get; set; }
    public Region Region { get; set; } = default!;  // TODO 
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public Dictionary<DateTime, NetworkObjectInfo> Infos { get; } = new();
}
