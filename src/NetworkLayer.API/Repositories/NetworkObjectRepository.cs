using Common.Services;
using Microsoft.Extensions.Options;
using NetworkLayer.API.Models;
using NetworkLayer.API.Options;

namespace NetworkLayer.API.Repositories;

public class NetworkObjectRepository : INetworkObjectRepository
{
    private readonly ILogger<NetworkObjectRepository> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly List<NetworkObject> _networkObjects;
    private readonly Random _random = new();

    private readonly IdGenerator _idGenerator = new();
    
    public NetworkObjectRepository(ILogger<NetworkObjectRepository> logger, IDateTimeProvider dateTimeProvider,
        IOptions<List<ExistingNoConfig>> existingNoConfigs)
    {
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;

        var now = _dateTimeProvider.Now;
        _networkObjects = existingNoConfigs.Value
            .Select(x => new NetworkObject
            {
                Id = _idGenerator.Next(),
                CreatedAt = now,
            }).ToList();
    }

    public IList<NetworkObject> GetAll()
    {
        // Randomize utilization
        foreach (var networkObject in _networkObjects)
        {
            networkObject.Utilization.CpuUtilization = _random.NextSingle();
            networkObject.Utilization.MemoryUtilization = _random.NextSingle();
            networkObject.Availability = _random.NextSingle();
        }

        return _networkObjects;
    }

    public void Create(NetworkObject networkObject)
    {
        // TODO assign other values
        networkObject.Id = _idGenerator.Next();
        networkObject.CreatedAt = _dateTimeProvider.Now;
        _networkObjects.Add(networkObject);
    }

    public bool Delete(int id)
    {
        return _networkObjects.RemoveAll(x => x.Id == id) > 0;
    }

    private class IdGenerator
    {
        private int _nextId = 1;
        public int Next() => _nextId++;
    }
}
