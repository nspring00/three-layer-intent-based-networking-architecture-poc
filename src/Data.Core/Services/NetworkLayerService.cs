using Common.Models;
using Data.Core.Clients;
using Data.Core.Models;
using Microsoft.Extensions.Logging;

namespace Data.Core.Services;

public class NetworkLayerService : INetworkLayerService
{
    private readonly ILogger<NetworkLayerService> _logger;
    private readonly INetworkObjectService _networkObjectService;
    private readonly NlGrpcClient _nlGrpcClient;

    public NetworkLayerService(
        ILogger<NetworkLayerService> logger,
        INetworkObjectService networkObjectService,
        NlGrpcClient nlGrpcClient)
    {
        _logger = logger;
        _networkObjectService = networkObjectService;
        _nlGrpcClient = nlGrpcClient;
    }

    public async Task FetchAllUpdates(int nlId, Region region, Uri uri)
    {
        var response = await _nlGrpcClient.FetchUpdates(uri);

        var updateTime = response.Timestamp.ToDateTime();

        _logger.LogInformation("Received {Count} updates from {NlId} at {UpdateTime}", response.NetworkObjects.Count,
            nlId, updateTime);
        
        foreach (var newNo in response.CreatedObjects)
        {
            if (newNo is null) continue;

            if (_networkObjectService.Exists(new NOId(region, newNo.Id)))
            {
                continue;
            }

            _networkObjectService.Create(new NetworkObject
            {
                NetworkLayerId = nlId,
                Region = region,
                Id = newNo.Id,
                Created = newNo.CreatedAt.ToDateTime(),
            });
        }

        foreach (var removedNo in response.RemovedObjects)
        {
            if (removedNo is null) continue;

            var result = _networkObjectService.Remove(new NOId(region, removedNo.Id));
            if (result)
            {
                _logger.LogInformation("Removed {NoId} from {NlId}", removedNo.Id, nlId);
            }
            else
            {
                _logger.LogWarning("Failed to remove {NoId} from {NlId}", removedNo.Id, nlId);
            }
        }

        foreach (var no in response.NetworkObjects)
        {
            if (no is null) continue;

            _networkObjectService.AddInfo(new NOId(region, no.Id), updateTime, new NetworkObjectInfo
            {
                Utilization = new Utilization
                {
                    CpuUtilization = no.Utilization.CpuUsage, MemoryUtilization = no.Utilization.MemoryUsage
                },
                Availability = no.Availability
            });
        }
    }
}
