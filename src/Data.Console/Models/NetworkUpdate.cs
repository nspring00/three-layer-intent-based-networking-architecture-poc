namespace Data.Console.Models;
public record NetworkUpdate
{
    public DateTime Timestamp { get; set; }
    public List<AddedNetworkObject> Added { get; } = new();
    public List<int> Removed { get; } = new();
    public Dictionary<int, NetworkObjectInfo> Updates { get; } = new();
}

public record AddedNetworkObject(int Id, string Application);
