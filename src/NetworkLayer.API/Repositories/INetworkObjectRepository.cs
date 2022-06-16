using NetworkLayer.API.Models;

namespace NetworkLayer.API.Repositories;

public interface INetworkObjectRepository
{
    public IList<NetworkObject> GetAll();
}
