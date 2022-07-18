using Common.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using NetworkLayer.API.Mappers;
using NetworkLayer.Grpc.NetworkObjects;

namespace NetworkLayer.API.Services;

public class NetworkObjectUpdateService : NetworkObjectUpdater.NetworkObjectUpdaterBase
{
    private readonly INetworkObjectService _networkObjectService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public NetworkObjectUpdateService(INetworkObjectService networkObjectService, IDateTimeProvider dateTimeProvider)
    {
        _networkObjectService = networkObjectService;
        _dateTimeProvider = dateTimeProvider;
    }

    public override Task<NetworkObjectUpdateResponse> GetUpdate(NetworkObjectUpdateRequest request, ServerCallContext context)
    {
        var response = new NetworkObjectUpdateResponse
        {
            Timestamp = Timestamp.FromDateTime(_dateTimeProvider.Now)
        };
        
        // TODO make this more refined
        response.CreatedObjects.AddRange(
            _networkObjectService.GetAll().Select(x => new NewNetworkObject
            {
                Id = x.Id,
                CreatedAt = Timestamp.FromDateTime(x.CreatedAt),
                Application = x.Application,
                Groups = { x.Groups }
            })
        );

        response.NetworkObjects.AddRange(_networkObjectService.GetAll()
            .Select(NetworkObjectMapper.MapNetworkObjectToGrpc));
        
        return Task.FromResult(response);
    }
}
