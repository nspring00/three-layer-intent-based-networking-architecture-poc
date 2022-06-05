using NetworkLayer.API.Models;
using NetworkLayer.API.Repositories;

namespace NetworkLayer.API.Services;

public class NetworkObjectService : INetworkObjectService
{
    private readonly INetworkObjectRepository _networkObjectRepository;


    public NetworkObjectService(INetworkObjectRepository networkObjectRepository)
    {
        _networkObjectRepository = networkObjectRepository;
    }

    public IList<NetworkObject> GetAll()
    {
        return _networkObjectRepository.GetAll();
    }
}
