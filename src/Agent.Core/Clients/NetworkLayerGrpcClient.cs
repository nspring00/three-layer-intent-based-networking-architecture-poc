using Agent.Core.Models;
using Common.Grpc;
using NetworkLayer.Grpc.Topology;

namespace Agent.Core.Clients;

public class NetworkLayerGrpcClient : CachedGrpcClient
{
    public async Task ScaleUp(Uri uri, IList<NetworkObjectCreateInfo> newNOs)
    {
        var channel = GetChannel(uri);
        var client = new NetworkTopologyUpdater.NetworkTopologyUpdaterClient(channel);

        var request = new ScaleUpRequest
        {
            NewNetworkObjects = { newNOs.Select(MapNewNetworkObject) }
        };
        
        await client.ScaleUpAsync(request);
    }

    public async Task ScaleDown(Uri uri, IList<int> removeIds)
    {
        var channel = GetChannel(uri);
        var client = new NetworkTopologyUpdater.NetworkTopologyUpdaterClient(channel);

        var request = new ScaleDownRequest { RemoveIds = { removeIds } };

        await client.ScaleDownAsync(request);
    }

    private static NewNetworkObject MapNewNetworkObject(NetworkObjectCreateInfo info)
    {
        return new NewNetworkObject
        {
            Id = info.Id,
            Application = info.Application,
            Groups = { info.Groups }
        };
    }
}
