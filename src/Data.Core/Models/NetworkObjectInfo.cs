namespace Data.Core.Models;
public class NetworkObjectInfo
{
    public Utilization Utilization { get; set; } = new();
    public float Availability { get; set; }
}
