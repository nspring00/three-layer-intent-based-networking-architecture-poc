using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using NetworkLayer.API.Mappers;
using NetworkLayer.API.Protos;

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
        
        response.NetworkObjects.AddRange(_networkObjectService.GetAll()
            .Select(NetworkObjectMapper.MapNetworkObjectToGrpc));
        
        return Task.FromResult(response);
    }
}
