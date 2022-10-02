using Common.Models;
using Data.Core.Models;

namespace Data.Core.Repositories;

// TODO make methods async
public interface INetworkObjectRepository
{
    void Create(NetworkObject networkObject);
    NetworkObject? Get(NOId id);
    IDictionary<int, IList<NetworkObject>>? GetAllForRegionByNlId(Region region);
    IDictionary<Region, IList<NetworkObject>> GetAll();
    bool Remove(NOId id);
}
