using Knowledge.API.Models;

namespace Knowledge.API.Repository;

public class CachedNetworkInfoRepository : INetworkInfoRepository
{
    private readonly List<NetworkDevice> _devices = new();

    // TODO remove after testing
    public CachedNetworkInfoRepository()
    {
        _devices.Add(new NetworkDevice(
            1, new Region("Vienna"), "WorkerApp1", 0.9f));
        _devices.Add(new NetworkDevice(
            2, new Region("Vienna"), "WorkerApp1", 0.9f));
        _devices.Add(new NetworkDevice(
            3, new Region("Vienna"), "WorkerApp1", 0.9f));
        _devices.Add(new NetworkDevice(
            4, new Region("Vienna"), "WorkerApp1", 0.9f));
        _devices.Add(new NetworkDevice(
            5, new Region("Vienna"), "WorkerApp1", 0.9f));
        
        _devices.Add(new NetworkDevice(
            1, new Region("Linz"), "WorkerApp1", 0.7f));
        _devices.Add(new NetworkDevice(
            2, new Region("Linz"), "WorkerApp1", 0.7f));
        _devices.Add(new NetworkDevice(
            3, new Region("Linz"), "WorkerApp1", 0.7f));
        _devices.Add(new NetworkDevice(
            4, new Region("Linz"), "WorkerApp1", 0.7f));
        _devices.Add(new NetworkDevice(
            5, new Region("Linz"), "WorkerApp1", 0.7f));
    }

    public void Add(NetworkDevice device)
    {
        _devices.Add(device);
    }
    
    public IList<NetworkDevice> GetForRegion(string region)
    {
        return _devices.Where(i => i.Region.Name == region).ToList();
    }
}
