using Common.Services;
using NetworkLayer.API.Models;
using NetworkLayer.API.Repositories;

namespace NetworkLayer.API.Services;

public class NetworkObjectService : INetworkObjectService
{
    private readonly INetworkObjectRepository _networkObjectRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    private DateTime _lastUpdate = DateTime.MinValue;

    public NetworkObjectService(INetworkObjectRepository networkObjectRepository, IDateTimeProvider dateTimeProvider)
    {
        _networkObjectRepository = networkObjectRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public IList<NetworkObject> GetAll()
    {
        return _networkObjectRepository.GetAll();
    }

    public (IList<NetworkObject>, IList<NetworkObject>) GetAllWithNew()
    {
        var all = GetAll();
        var now = _dateTimeProvider.Now;
        var newObjects = all.Where(x => x.CreatedAt > _lastUpdate && x.CreatedAt <= now).ToList();
        _lastUpdate = now;
        
        return (all, newObjects);
    }
}
