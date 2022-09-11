using Common.Models;
using Grpc.Core;
using Knowledge.API.Models;
using Knowledge.API.Repository;
using Knowledge.Grpc.NetworkInfoUpdate;
using MediatR;
using WorkloadInfo = Knowledge.Grpc.NetworkInfoUpdate.WorkloadInfo;

namespace Knowledge.API.Services;

public class NetworkInfoUpdateService : NetworkInfoUpdater.NetworkInfoUpdaterBase
{
    private readonly ILogger<NetworkInfoUpdateService> _logger;
    private readonly IWorkloadRepository _workloadRepository;
    private readonly IMediator _mediator;

    public NetworkInfoUpdateService(
        ILogger<NetworkInfoUpdateService> logger,
        IWorkloadRepository workloadRepository,
        IMediator mediator)
    {
        _logger = logger;
        _workloadRepository = workloadRepository;
        _mediator = mediator;
    }

    public override Task<NetworkInfoUpdateResponse> Update(
        NetworkInfoUpdateRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation($"Received update for {request.RegionUpdates.Count} regions");

        foreach (var regionUpdate in request.RegionUpdates)
        {
            var region = new Region(regionUpdate.RegionName);
            Models.WorkloadInfo update = MapWorkloadInfo(regionUpdate.WorkloadInfo);
            _workloadRepository.Add(region, update);
        }

        // Fire and forget notification event
        var notification = new WorkloadInfoAddedNotification(request.Timestamp.ToDateTime(),
            request.RegionUpdates.Select(x => new Region(x.RegionName)).ToList());
        
        _mediator.Publish(notification);

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
