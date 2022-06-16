using Grpc.Core;
using Knowledge.API.Models;
using Knowledge.API.Repository;
using Knowledge.Grpc.NetworkInfoUpdate;
using Utilization = Knowledge.API.Models.Utilization;

namespace Knowledge.API.Services;

public class NetworkInfoUpdateService : NetworkInfoUpdater.NetworkInfoUpdaterBase
{
    private readonly ILogger<NetworkInfoUpdateService> _logger;
    private readonly INetworkInfoRepository _networkInfoRepository;

    public NetworkInfoUpdateService(
        ILogger<NetworkInfoUpdateService> logger,
        INetworkInfoRepository networkInfoRepository)
    {
        _logger = logger;
        _networkInfoRepository = networkInfoRepository;
    }

    public override Task<NetworkInfoUpdateResponse> Update(
        NetworkInfoUpdateRequest request,
        ServerCallContext context)
    {
        foreach (var regionUpdate in request.RegionUpdates)
        {
            var regionName = regionUpdate.RegionName!;
            foreach (var addedNo in regionUpdate.Added)
            {
                AddDevice(regionName, addedNo);
            }
            
            foreach (var removedId in regionUpdate.Removed)
            {
                _networkInfoRepository.Remove(regionName, removedId);
            }

            foreach (var workloadUpdate in regionUpdate.WorkloadUpdates)
            {
                HandleWorkloadUpdate(regionName, workloadUpdate);
            }
        }

        return Task.FromResult(new NetworkInfoUpdateResponse());
    }

    private void HandleWorkloadUpdate(string regionName, WorkloadUpdate workloadUpdate)
    {
        var device = _networkInfoRepository.Get(regionName, workloadUpdate.NetworkObjectId);
        if (device == null)
        {
            _logger.LogError($"No device {regionName} {workloadUpdate.NetworkObjectId} exists");
            return;
            // TODO throw NotFound or something???
            //AddDevice(regionName, workloadUpdate.NetworkObjectId, new Utilization
            //{
            //    CpuUtilization = workloadUpdate.Utilization.CpuUtilization,
            //    MemoryUtilization = workloadUpdate.Utilization.MemoryUtilization
            //});
            //return;
        }

        device.Utilization.CpuUtilization = workloadUpdate.Utilization.CpuUtilization;
        device.Utilization.MemoryUtilization = workloadUpdate.Utilization.MemoryUtilization;
    }

    private void AddDevice(string region, AddedNetworkObject no, Utilization? utilization = null)
    {
        utilization ??= new Utilization
        {
            CpuUtilization = -1f, MemoryUtilization = -1f
        };

        _networkInfoRepository.Add(
            new NetworkDevice(no.Id, new Region(region), no.Application, utilization));
    }
}
