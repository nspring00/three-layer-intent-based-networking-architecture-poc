namespace Data.Console.Models;
public class NetworkUpdate
{
    public DateTime Timestamp { get; set; }
    public List<NOId> Added { get; } = new();
    public List<NOId> Removed { get; } = new();
    public Dictionary<NOId, NetworkObjectInfo> Updates { get; } = new();
}
