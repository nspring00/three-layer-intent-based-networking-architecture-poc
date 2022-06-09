namespace Data.Console.Models;
public record NetworkUpdate
{
    public DateTime Timestamp { get; set; }
    public List<int> Added { get; } = new();
    public List<int> Removed { get; } = new();
    public Dictionary<int, NetworkObjectInfo> Updates { get; } = new();
}
