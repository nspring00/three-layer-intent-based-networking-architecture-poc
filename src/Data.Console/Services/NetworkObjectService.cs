using Data.Console.Models;
using Data.Console.Repositories;
using Microsoft.Extensions.Logging;

namespace Data.Console.Services;
public class NetworkObjectService : INetworkObjectService
{
    private readonly ILogger<NetworkObjectService> _logger;
    private readonly INetworkObjectRepository _networkObjectRepository;

    public NetworkObjectService(
        ILogger<NetworkObjectService> logger, 
        INetworkObjectRepository networkObjectRepository)
    {
        _logger = logger;
        _networkObjectRepository = networkObjectRepository;
    }

    public void Create(NetworkObject networkObject)
    {
        _networkObjectRepository.Create(networkObject);
    }
    
    public void AddInfo(NOId id, DateTime updateTime, NetworkObjectInfo info)
    {
        var no = _networkObjectRepository.Get(id);
        if (no is null)
        {
            _logger.LogWarning("NetworkObject not found: {Id}", id);
            return;
        }

        var lastUpdate = no.LastUpdate ?? no.Created;
        var elapsed = updateTime - lastUpdate;
        no.Infos.Add(elapsed, info);
    }

    public NetworkUpdate AggregateUpdates(TimeSpan timeSpan)
    {
        throw new NotImplementedException();
    }
}
