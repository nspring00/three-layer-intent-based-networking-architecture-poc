using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using NetworkLayer.API.Mappers;
using NetworkLayer.Grpc;

namespace NetworkLayer.API.Services;

public class NetworkObjectUpdateService : NetworkObjectUpdater.NetworkObjectUpdaterBase
{
    private readonly INetworkObjectService _networkObjectService;

    public NetworkObjectUpdateService(INetworkObjectService networkObjectService)
    {
        _networkObjectService = networkObjectService;
    }

    public override Task<NetworkObjectUpdateResponse> GetUpdate(NetworkObjectUpdateRequest request, ServerCallContext context)
    {
        var response = new NetworkObjectUpdateResponse
        {
            Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
        };
        
        // TODO make this more refined
        response.CreatedObjects.AddRange(
            _networkObjectService.GetAll().Select(x => new NewNetworkObject
            {
                Id = x.Id,
                CreatedAt = Timestamp.FromDateTime(x.CreatedAt)
            })
        );

        response.NetworkObjects.AddRange(_networkObjectService.GetAll()
            .Select(NetworkObjectMapper.MapNetworkObjectToGrpc));
        
        return Task.FromResult(response);
    }
}
