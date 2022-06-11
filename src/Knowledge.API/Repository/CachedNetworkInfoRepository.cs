using Knowledge.API.Models;

namespace Knowledge.API.Repository;

public class CachedNetworkInfoRepository : INetworkInfoRepository
{
    private readonly ILogger<CachedNetworkInfoRepository> _logger;
    private readonly List<NetworkDevice> _devices = new();

    // TODO remove after testing
    public CachedNetworkInfoRepository(ILogger<CachedNetworkInfoRepository> logger)
    {
        _logger = logger;
        //_devices.Add(new NetworkDevice(
        //    1, new Region("Vienna"), "WorkerApp1", new Utilization
        //    {
        //        CpuUtilization = 0.9f, MemoryUtilization = 0.9f
        //    }));
        //_devices.Add(new NetworkDevice(
        //    2, new Region("Vienna"), "WorkerApp1", new Utilization
        //    {
        //        CpuUtilization = 0.9f, MemoryUtilization = 0.9f
        //    }));
        //_devices.Add(new NetworkDevice(
        //    3, new Region("Vienna"), "WorkerApp1", new Utilization
        //    {
        //        CpuUtilization = 0.9f, MemoryUtilization = 0.9f
        //    }));
        //_devices.Add(new NetworkDevice(
        //    4, new Region("Vienna"), "WorkerApp1", new Utilization
        //    {
        //        CpuUtilization = 0.9f, MemoryUtilization = 0.9f
        //    }));
        //_devices.Add(new NetworkDevice(
        //    5, new Region("Vienna"), "WorkerApp1", new Utilization
        //    {
        //        CpuUtilization = 0.9f, MemoryUtilization = 0.9f
        //    }));
        //_devices.Add(new NetworkDevice(
        //    1, new Region("Linz"), "WorkerApp1", new Utilization
        //    {
        //        CpuUtilization = 0.7f, MemoryUtilization = 0.7f
        //    }));
        //_devices.Add(new NetworkDevice(
        //    2, new Region("Linz"), "WorkerApp1", new Utilization
        //    {
        //        CpuUtilization = 0.7f, MemoryUtilization = 0.7f
        //    }));
        //_devices.Add(new NetworkDevice(
        //    3, new Region("Linz"), "WorkerApp1", new Utilization
        //    {
        //        CpuUtilization = 0.7f, MemoryUtilization = 0.7f
        //    }));
        //_devices.Add(new NetworkDevice(
        //    4, new Region("Linz"), "WorkerApp1", new Utilization
        //    {
        //        CpuUtilization = 0.7f, MemoryUtilization = 0.7f
        //    }));
        //_devices.Add(new NetworkDevice(
        //    5, new Region("Linz"), "WorkerApp1", new Utilization
        //    {
        //        CpuUtilization = 0.7f, MemoryUtilization = 0.7f
        //    }));
    }

    public void Add(NetworkDevice device)
    {
        _logger.LogInformation($"Adding device {device}");
        if (_devices.Any(d => d.Id == device.Id && d.Region.Name == device.Region.Name))
        {
            _logger.LogWarning($"Device {device} already exists");
            return;
        }
        
        _devices.Add(device);
    }

    public bool Remove(string region, int id)
    {
        _logger.LogInformation($"Removing device {region} {id}");
        var removed = _devices.RemoveAll(d => d.Id == id && d.Region.Name == region);
        return removed > 0;
    }

    public NetworkDevice? Get(string region, int id)
    {
        return _devices.FirstOrDefault(d => d.Id == id && d.Region.Name == region);
    }

    public IList<NetworkDevice> GetForRegion(string region)
    {
        return _devices.Where(i => i.Region.Name == region).ToList();
    }
}
