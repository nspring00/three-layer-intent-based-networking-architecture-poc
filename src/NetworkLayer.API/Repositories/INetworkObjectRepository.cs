using NetworkLayer.API.Models;

namespace NetworkLayer.API.Repositories;

public interface INetworkObjectRepository
{
    IList<NetworkObject> GetAll();
    IList<NetworkObject> GetCreated();
    IList<(NetworkObject, DateTime)> GetRemoved();
    void Create(NetworkObject networkObject);
    bool Delete(int id);
    void ResetCreateDelete();
}
