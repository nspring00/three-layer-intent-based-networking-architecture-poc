using Common.Models;
using Knowledge.API.Models;

namespace Knowledge.API.Repository;

public interface INetworkInfoRepository
{
    NetworkDevice? Get(Region region, int id);
    IList<NetworkDevice> GetForRegion(Region region);
    void Add(NetworkDevice networkDevice);
    bool Remove(Region region, int id);
}
