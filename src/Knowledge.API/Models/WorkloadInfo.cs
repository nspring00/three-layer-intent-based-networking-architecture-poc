namespace Knowledge.API.Models;

public class WorkloadInfo
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public int DeviceCount { get; set; }
    public float AvgEfficiency { get; set; }
    public float AvgAvailability { get; set; }
}
