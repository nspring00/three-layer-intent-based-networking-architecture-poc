using NetworkLayer.API.Models;

namespace NetworkLayer.API.Services;

public interface INetworkObjectService
{
    IList<NetworkObject> GetAll();
    (IList<NetworkObject>, IList<NetworkObject>) GetAllWithNew();
}
