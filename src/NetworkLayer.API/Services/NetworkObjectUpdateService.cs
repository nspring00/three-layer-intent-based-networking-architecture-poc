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
                Groups =
                {
                    x.Groups
                }
            })
        );

        response.NetworkObjects.AddRange(_networkObjectService.GetAll()
            .Select(NetworkObjectMapper.MapNetworkObjectToGrpc));

        _logger.LogInformation("Fetched update: New {NewCount} Total {TotalCount}", response.CreatedObjects.Count,
            response.NetworkObjects.Count);

        return Task.FromResult(response);
    }
}
