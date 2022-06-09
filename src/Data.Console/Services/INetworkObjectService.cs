using Data.Console.Models;

namespace Data.Console.Services;

public interface INetworkObjectService
{
    void Create(NetworkObject networkObject);
    void AddInfo(NOId id, DateTime updateTime, NetworkObjectInfo info);
    IDictionary<Region, NetworkUpdate> AggregateUpdates(DateTime from, DateTime to);
    void Reset();
}
