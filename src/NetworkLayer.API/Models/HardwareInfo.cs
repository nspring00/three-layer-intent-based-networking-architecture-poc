namespace NetworkLayer.API.Models;

public record HardwareInfo
{   
    public int CpuOps { get; set; }
    public int MemorySize { get; set; }
}
