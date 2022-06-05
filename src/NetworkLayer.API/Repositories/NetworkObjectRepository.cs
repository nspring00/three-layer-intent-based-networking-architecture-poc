using NetworkLayer.API.Models;

namespace NetworkLayer.API.Repositories;

public class NetworkObjectRepository : INetworkObjectRepository
{
    private readonly List<NetworkObject> _randomNOs;
    private readonly Random _random = new ();

    public NetworkObjectRepository()
    {
        _randomNOs = new List<NetworkObject>
        {
            new()
            {
                Id = 1, Application = "Application1"
            },
            new()
            {
                Id = 2, Application = "Application1"
            },
            new()
            {
                Id = 3, Application = "Application2"
            },
            new()
            {
                Id = 4, Application = "Application1"
            },
            new()
            {
                Id = 5, Application = "Application2"
            },
            new()
            {
                Id = 7, Application = "Application2"
            }
        };
    }

    public IList<NetworkObject> GetAll()
    {
        // Randomize utilization
        foreach (var networkObject in _randomNOs)
        {
            networkObject.Utilization.CpuUtilization = _random.NextSingle();
            networkObject.Utilization.MemoryUtilization = _random.NextSingle();
        }

        return _randomNOs;
    }
}
