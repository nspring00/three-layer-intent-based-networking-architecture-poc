using Data.Console.Models;
using Microsoft.Extensions.Logging;

namespace Data.Console.Repositories;

internal class NetworkObjectRepository : INetworkObjectRepository
{
    private readonly ILogger<NetworkObjectRepository> _logger;
    private readonly Dictionary<Region, List<NetworkObject>> _networkObjects = new();

    public NetworkObjectRepository(ILogger<NetworkObjectRepository> logger)
    {
        _logger = logger;
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

        _networkObjects[networkObject.Region] = new List<NetworkObject>
        {
            networkObject
        };
    }

    public NetworkObject? Get(NOId noId)
    {
        var (region, id) = noId;
        _networkObjects.TryGetValue(region, out var networkObjects);
        return networkObjects?.FirstOrDefault(x => x.Id == id);
    }
}
