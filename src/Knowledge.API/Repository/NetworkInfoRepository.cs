using Knowledge.API.Models;

namespace Knowledge.API.Repository;

public class NetworkInfoRepository
{
    private readonly List<NetworkDevice> _devices = new();

    // TODO remove after testing
    public NetworkInfoRepository()
    {
        _devices.Add(new NetworkDevice(
            1, new Region("Vienna"), "WorkerApp1", 0.98f));
        //_devices.Add(new NetworkDevice(
        //    2, new Region("Vienna"), "WorkerApp1", 0.93f));
        //_devices.Add(new NetworkDevice(
        //    4, new Region("Vienna"), "WorkerApp1", 0.88f));
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
