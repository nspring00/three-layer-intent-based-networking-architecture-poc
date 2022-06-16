using Data.Console.Clients;
using Data.Console.Models;
using Microsoft.Extensions.Logging;

namespace Data.Console.Services;
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

    public async Task FetchAllUpdates(Region region, Uri uri)
    {
        var response = await _nlGrpcClient.FetchUpdates(uri);

        var updateTime = response.Timestamp.ToDateTime();
        foreach (var newNo in response.CreatedObjects)
        {
            if (newNo is null) continue;

            if (_networkObjectService.Exists(new NOId(region, newNo.Id)))
            {
                continue;
            }

            _networkObjectService.Create(new NetworkObject
            {
                Region = region,
                Id = newNo.Id,
                Created = newNo.CreatedAt.ToDateTime(),
                Application = newNo.Application
            });
        }

        foreach (var no in response.NetworkObjects)
        {
            if (no is null) continue;

            _networkObjectService.AddInfo(new NOId(region, no.Id), updateTime, new NetworkObjectInfo
            {
                Utilization = new Utilization
                {
                    CpuUtilization = no.Utilization.CpuUsage,
                    MemoryUtilization = no.Utilization.MemoryUsage
                },
                Availability = no.Availability
            });
        }
    }
}
