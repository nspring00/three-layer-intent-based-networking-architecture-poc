using Common.Services;
using NetworkLayer.API.Models;

namespace NetworkLayer.API.Repositories;

public class NetworkObjectRepository : INetworkObjectRepository
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly List<NetworkObject> _randomNOs;
    private readonly Random _random = new();

    private readonly IdGenerator _idGenerator = new();

    public NetworkObjectRepository(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
        _randomNOs = new List<NetworkObject>
        {
            new()
            {
                Id = _idGenerator.Next(),
                Application = "Application1",
                Groups = { "Group1" },
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = _idGenerator.Next(),
                Application = "Application1",
                Groups = { "Group1" },
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = _idGenerator.Next(),
                Application = "Application2",
                Groups = { "Group2" },
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = _idGenerator.Next(),
                Application = "Application1",
                Groups = { "Group2" },
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = _idGenerator.Next(),
                Application = "Application2",
                Groups = { "Group1", "Group2" },
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = _idGenerator.Next(),
                Application = "Application2",
                Groups = { "Group1", "Group2" },
                CreatedAt = DateTime.UtcNow
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
            networkObject.Availability = _random.NextSingle();
        }

        return _randomNOs;
    }

    public void Create(NetworkObject networkObject)
    {
        // TODO assign other values
        networkObject.Id = _idGenerator.Next();
        networkObject.CreatedAt = _dateTimeProvider.Now;
        _randomNOs.Add(networkObject);
    }

    public bool Delete(int id)
    {
        return _randomNOs.RemoveAll(x => x.Id == id) > 0;
    }

    private class IdGenerator
    {
        private int _nextId = 1;
        public int Next() => _nextId++;
    }
}
