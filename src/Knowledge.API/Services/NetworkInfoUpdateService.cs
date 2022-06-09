using Grpc.Core;
using Knowledge.API.Models;
using Knowledge.API.Repository;
using Knowledge.Grpc.NetworkInfoUpdate;
using Utilization = Knowledge.API.Models.Utilization;

namespace Knowledge.API.Services;

public class NetworkInfoUpdateService : NetworkInfoUpdater.NetworkInfoUpdaterBase
{
    private readonly INetworkInfoRepository _networkInfoRepository;

    public NetworkInfoUpdateService(INetworkInfoRepository networkInfoRepository)
    {
        _networkInfoRepository = networkInfoRepository;
    }

    public override Task<NetworkInfoUpdateResponse> Update(
        NetworkInfoUpdateRequest request,
        ServerCallContext context)
    {
        foreach (var regionUpdate in request.RegionUpdates)
        {
            var regionName = regionUpdate.RegionName!;
            foreach (var topologyUpdate in regionUpdate.TopologyUpdates)
            {
                HandleTopologyUpdate(regionName, topologyUpdate);
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
            // TODO throw NotFound or something???
            AddDevice(regionName, workloadUpdate.NetworkObjectId, new Utilization
            {
                CpuUtilization = workloadUpdate.Utilization.CpuUtilization,
                MemoryUtilization = workloadUpdate.Utilization.MemoryUtilization
            });
            return;
        }

        device.Utilization.CpuUtilization = workloadUpdate.Utilization.CpuUtilization;
        device.Utilization.MemoryUtilization = workloadUpdate.Utilization.MemoryUtilization;
    }

    private void HandleTopologyUpdate(string regionName, TopologyUpdate topologyUpdate)
    {
        // TODO check first that all objects to remove exist and return error otherwise

        switch (topologyUpdate.UpdateType)
        {
            case TopologyUpdate.Types.NetworkObjectUpdateType.Add:
                AddDevice(regionName, topologyUpdate.NetworkObjectId);
                break;
            case TopologyUpdate.Types.NetworkObjectUpdateType.Remove:
                _networkInfoRepository.Remove(regionName, topologyUpdate.NetworkObjectId);
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    $"Unsupported topology update type {topologyUpdate.UpdateType}");
        }
    }

    private void AddDevice(string region, int id, Utilization? utilization = null)
    {
        utilization ??= new Utilization
        {
            CpuUtilization = -1f, MemoryUtilization = -1f
        };

        _networkInfoRepository.Add(
            new NetworkDevice(id, new Region(region), "TODO", utilization));
    }
}
