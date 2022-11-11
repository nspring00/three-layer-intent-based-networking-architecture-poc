using Common.Models;
using Data.Core.Models;

namespace Data.Core.Services;

public interface INetworkObjectService
{
    void Create(NetworkObject networkObject);
    bool Remove(NOId id);
    void AddInfo(NOId id, DateTime updateTime, NetworkObjectInfo info);
    bool Exists(NOId id);
    IDictionary<Region, NetworkUpdate> AggregateUpdates(DateTime from, DateTime to);
    void Reset();
}
