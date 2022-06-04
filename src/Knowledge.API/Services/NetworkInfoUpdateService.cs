using Grpc.Core;
using Knowledge.API.Models;
using Knowledge.API.Repository;

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
        }

        return Task.FromResult(new NetworkInfoUpdateResponse());
    }

    private void HandleTopologyUpdate(string regionName, TopologyUpdate topologyUpdate)
    {
        // TODO check first that all objects to remove exist and return error otherwise
        
        switch (topologyUpdate.UpdateType)
        {
            case TopologyUpdate.Types.NetworkObjectUpdateType.Add:
                _networkInfoRepository.Add(
                    new NetworkDevice(topologyUpdate.NetworkObjectId, new Region(regionName), "TODO", -1f));
                break;
            case TopologyUpdate.Types.NetworkObjectUpdateType.Remove:
                _networkInfoRepository.Remove(regionName, topologyUpdate.NetworkObjectId);
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    $"Unsupported topology update type {topologyUpdate.UpdateType}");
        }
    }
}
