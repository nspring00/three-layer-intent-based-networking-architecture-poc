using Common.Services;
using Microsoft.Extensions.Options;
using NetworkLayer.API.Models;
using NetworkLayer.API.Options;

namespace NetworkLayer.API.Repositories;

public class RandomNetworkObjectRepository : INetworkObjectRepository
{
    private readonly ILogger<RandomNetworkObjectRepository> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;

    private readonly Random _random = new();
    
    private readonly List<(int, DateTime)> _networkObjects = new();
    private readonly List<(int, DateTime)> _created = new();
    private readonly List<(int, DateTime, DateTime)> _deleted = new();

    public RandomNetworkObjectRepository(
        ILogger<RandomNetworkObjectRepository> logger,
        IDateTimeProvider dateTimeProvider,
        IOptions<SimulationConfig> config)
    {
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;

        var now = dateTimeProvider.Now;
        for (var x = 1; x <= config.Value.InitialCount; x++)
        {
            _networkObjects.Add((x, now));
            _created.Add((x, now));
        }
    }
    
    public IList<NetworkObject> GetAll()
    {
        return _networkObjects.Select(x => new NetworkObject
        {
            Id = x.Item1,
            CreatedAt = x.Item2,
            Utilization = new Utilization
            {
                CpuUtilization = _random.NextSingle(),
                MemoryUtilization = _random.NextSingle()
            },
            Availability = _random.NextSingle()
        }).ToList();
    }

    public IList<NetworkObject> GetCreated()
    {
        return _created.Select(x => new NetworkObject
        {
            Id = x.Item1, CreatedAt = x.Item2
        }).ToList();
    }

    public IList<(NetworkObject, DateTime)> GetRemoved()
    {
        return _deleted.Select(x => (new NetworkObject
        {
            Id = x.Item1, CreatedAt = x.Item2
        }, x.Item3)).ToList();
    }

    public void Create(NetworkObject networkObject)
    {
        var now = _dateTimeProvider.Now;
        _networkObjects.Add((networkObject.Id, now));
        _created.Add((networkObject.Id, now));
    }

    public bool Delete(int id)
    {
        if (_networkObjects.All(x => x.Item1 != id))
        {
            return false;
        }
        var item = _networkObjects.First(x => x.Item1 == id);
        _networkObjects.Remove(item);
        _deleted.Add((id, item.Item2, _dateTimeProvider.Now));
        return true;
    }

    public void ResetCreateDelete()
    {
        _created.Clear();
        _deleted.Clear();
    }
}
