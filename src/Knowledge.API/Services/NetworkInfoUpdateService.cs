using Common.Models;
using Grpc.Core;
using Knowledge.API.Repository;
using Knowledge.Grpc.NetworkInfoUpdate;
using WorkloadInfo = Knowledge.Grpc.NetworkInfoUpdate.WorkloadInfo;

namespace Knowledge.API.Services;

public class NetworkInfoUpdateService : NetworkInfoUpdater.NetworkInfoUpdaterBase
{
    private readonly ILogger<NetworkInfoUpdateService> _logger;
    private readonly IWorkloadRepository _workloadRepository;

    public NetworkInfoUpdateService(
        ILogger<NetworkInfoUpdateService> logger,
        IWorkloadRepository workloadRepository)
    {
        _logger = logger;
        _workloadRepository = workloadRepository;
    }

    public override Task<NetworkInfoUpdateResponse> Update(
        NetworkInfoUpdateRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation($"Received update for {request.RegionUpdates.Count} regions");

        // TODO use timestamp here?
        foreach (var regionUpdate in request.RegionUpdates)
        {
            var region = new Region(regionUpdate.RegionName);
            Models.WorkloadInfo update = MapWorkloadInfo(regionUpdate.WorkloadInfo);
            _workloadRepository.Add(region, update);
        }

        return Task.FromResult(new NetworkInfoUpdateResponse());
    }

    private static Models.WorkloadInfo MapWorkloadInfo(WorkloadInfo info)
    {
        return new Models.WorkloadInfo
        {
            DeviceCount = info.DeviceCount,
            AvgEfficiency = info.AvgEfficiency,
            AvgAvailability = info.AvgAvailability
        };
    }
}
