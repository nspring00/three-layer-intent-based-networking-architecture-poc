using Common.Models;
using Data.Console.Models;

namespace Data.Console.Repositories;

// TODO make methods async
public interface INetworkObjectRepository
{
    void Create(NetworkObject networkObject);
    NetworkObject? Get(NOId id);
    IDictionary<int, IList<NetworkObject>>? GetAllForRegionByNlId(Region region);
    IDictionary<Region, IList<NetworkObject>> GetAll();
    bool Remove(NOId id);
}
