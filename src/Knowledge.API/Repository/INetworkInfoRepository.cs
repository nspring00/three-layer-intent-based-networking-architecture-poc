using Knowledge.API.Models;

namespace Knowledge.API.Repository;

public interface INetworkInfoRepository
{
    NetworkDevice? Get(string region, int id);
    IList<NetworkDevice> GetForRegion(string region);
    void Add(NetworkDevice networkDevice);
    bool Remove(string region, int id);
}
