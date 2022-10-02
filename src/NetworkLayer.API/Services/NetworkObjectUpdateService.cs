using Common.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using NetworkLayer.API.Mappers;
using NetworkLayer.Grpc.NetworkObjects;

namespace NetworkLayer.API.Services;

public class NetworkObjectUpdateService : NetworkObjectUpdater.NetworkObjectUpdaterBase
{
    private readonly ILogger<NetworkObjectUpdateService> _logger;
    private readonly INetworkObjectService _networkObjectService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public NetworkObjectUpdateService(ILogger<NetworkObjectUpdateService> logger,
        INetworkObjectService networkObjectService, IDateTimeProvider dateTimeProvider)
    {
        _logger = logger;
        _networkObjectService = networkObjectService;
        _dateTimeProvider = dateTimeProvider;
    }

    public override Task<NetworkObjectUpdateResponse> GetUpdate(NetworkObjectUpdateRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("Update requested");

        var now = _dateTimeProvider.Now;
        var response = new NetworkObjectUpdateResponse
        {
            Timestamp = Timestamp.FromDateTime(now)
        };

        var (allNos, newNos) = _networkObjectService.GetAllWithNew();
        
        response.CreatedObjects.AddRange(
            newNos.Select(x => new NewNetworkObject
            {
                Id = x.Id,
                CreatedAt = Timestamp.FromDateTime(x.CreatedAt)
            })
        );

        response.NetworkObjects.AddRange(allNos
            .Select(NetworkObjectMapper.MapNetworkObjectToGrpc));

        _logger.LogInformation("Fetched update: New {NewCount} Total {TotalCount}", response.CreatedObjects.Count,
            response.NetworkObjects.Count);

        return Task.FromResult(response);
    }
}
