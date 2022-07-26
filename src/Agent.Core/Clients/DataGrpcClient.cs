using Agent.Core.Models;
using Common.Grpc;
using Common.Models;
using Data.Grpc.Topology;

namespace Agent.Core.Clients;
public class DataGrpcClient : CachedGrpcClient
{
    public async Task<IDictionary<Region, IDictionary<int, NlManagerTopology>>> GetTopologyForRegionsAsync(Uri uri, IEnumerable<Region> regions)
    {
        var channel = GetChannel(uri);
        var client = new TopologyService.TopologyServiceClient(channel);

        var request = new RegionTopologyRequest
        {
            RegionNames = { regions.Select(x => x.Name).ToList() }
        };

        var response = await client.GetTopologyForRegionsAsync(request);

        return response.RegionTopologies
            .ToDictionary(x => new Region(x.RegionName), MapRegionTopology);
    }

    private static IDictionary<int, NlManagerTopology> MapRegionTopology(RegionTopology topology)
    {
        return topology.NlManagers.ToDictionary(x => x.Id,
            x => new NlManagerTopology(x.Id, new Uri(x.Uri), x.ActiveDevices.Select(MapDevice).ToList()));
    }

    private static NetworkDevice MapDevice(DeviceInfo device)
    {
        return new NetworkDevice(device.Id, device.Uptime.ToTimeSpan());
    }
}
