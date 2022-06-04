using Knowledge.API.Models;

namespace Knowledge.API.Repository;

public interface INetworkInfoRepository
{
    IList<NetworkDevice> GetForRegion(string region);
}
