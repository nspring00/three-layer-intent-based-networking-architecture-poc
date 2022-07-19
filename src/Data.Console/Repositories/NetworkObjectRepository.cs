using Common.Models;
using Data.Console.Models;
using Microsoft.Extensions.Logging;

namespace Data.Console.Repositories;

public class NetworkObjectRepository : INetworkObjectRepository
{
    private readonly ILogger<NetworkObjectRepository> _logger;
    private readonly Dictionary<Region, IList<NetworkObject>> _networkObjects = new();

    private const bool InsertTestData = false;

    public NetworkObjectRepository(ILogger<NetworkObjectRepository> logger)
    {
        _logger = logger;

        if (InsertTestData)
        {
            _networkObjects.Add(new Region("Vienna"),
                new List<NetworkObject>
                {
                    new()
                    {
                        Id = 1,
                        Region = new Region("Vienna"),
                        NetworkLayerId = 1,
                        Application = "Application1",
                        Created = DateTime.UtcNow.AddMinutes(-2)
                    },
                    new()
                    {
                        Id = 2,
                        Region = new Region("Vienna"),
                        NetworkLayerId = 1,
                        Application = "Application1",
                        Created = DateTime.UtcNow.AddMinutes(-1)
                    },
                    new()
                    {
                        Id = 3,
                        Region = new Region("Vienna"),
                        NetworkLayerId = 1,
                        Application = "Application1",
                        Created = DateTime.UtcNow.AddMinutes(-2)
                    },
                    //new()
                    //{
                    //    Id = 4,
                    //    Region = new Region("Vienna"),
                    //    NetworkLayerId = 2,
                    //    Application = "Application1",
                    //    Created = DateTime.UtcNow.AddMinutes(-3)
                    //},
                    //new()
                    //{
                    //    Id = 5,
                    //    Region = new Region("Vienna"),
                    //    NetworkLayerId = 2,
                    //    Application = "Application1",
                    //    Created = DateTime.UtcNow.AddMinutes(0)
                    //}
                });

            _networkObjects.Add(new Region("Linz"),
                new List<NetworkObject>
                {
                    new()
                    {
                        Id = 6,
                        Region = new Region("Linz"),
                        NetworkLayerId = 3,
                        Application = "Application2",
                        Created = DateTime.UtcNow.AddMinutes(-1)
                    },
                    //new()
                    //{
                    //    Id = 7,
                    //    Region = new Region("Linz"),
                    //    NetworkLayerId = 4,
                    //    Application = "Application2",
                    //    Created = DateTime.UtcNow.AddMinutes(-3)
                    //}
                });
        }
    }

    public void Create(NetworkObject networkObject)
    {
        var noId = new NOId(networkObject.Region, networkObject.Id);
        if (Get(noId) is not null)
        {
            _logger.LogError("NetworkObject {Id} already exists", noId);
        }

        if (_networkObjects.ContainsKey(networkObject.Region))
        {
            _networkObjects[networkObject.Region].Add(networkObject);
            return;
        }

        _networkObjects[networkObject.Region] = new List<NetworkObject> { networkObject };
    }

    public NetworkObject? Get(NOId noId)
    {
        var (region, id) = noId;
        _networkObjects.TryGetValue(region, out var networkObjects);
        return networkObjects?.FirstOrDefault(x => x.Id == id);
    }

    public IDictionary<int, IList<NetworkObject>>? GetAllForRegionByNlId(Region region)
    {
        if (!_networkObjects.ContainsKey(region))
        {
            return null;
        }

        return _networkObjects[region]
            .GroupBy(x => x.NetworkLayerId)
            .ToDictionary(
                x => x.Key, 
                x => x.Select(no => no).ToList() as IList<NetworkObject>);
    }

    public IDictionary<Region, IList<NetworkObject>> GetAll()
    {
        return _networkObjects;
    }

    public bool Remove(NOId id)
    {
        var nos = _networkObjects[id.Region];
        var toRemove = nos.FirstOrDefault(x => x.Id == id.Id);
        return toRemove is not null && nos.Remove(toRemove);
    }
}
