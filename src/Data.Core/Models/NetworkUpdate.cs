namespace Data.Core.Models;
public record NetworkUpdate
{
    public int DeviceCount { get; set; }
    public float AvgEfficiency { get; set; }
    public float AvgAvailability { get; set; }
}

public record AddedNetworkObject(int Id);
