namespace Data.Core.Models;

public record Utilization
{
    public float CpuUtilization { get; set; }
    public float MemoryUtilization { get; set; }
}
