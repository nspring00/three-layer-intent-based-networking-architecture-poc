using NetworkLayer.API.Models;

namespace NetworkLayer.API.Repositories;

public interface INetworkObjectRepository
{
    IList<NetworkObject> GetAll();
    void Create(NetworkObject networkObject);
    bool Delete(int id);
}
