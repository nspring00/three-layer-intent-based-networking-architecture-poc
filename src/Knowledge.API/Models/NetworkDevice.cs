using Common.Models;

namespace Knowledge.API.Models;

public record NetworkDevice(int Id, Region Region, string Application, Utilization Utilization);

public record Utilization
{
    public float CpuUtilization { get; set; } = default!;
    public float MemoryUtilization { get; set; } = default!;
}
