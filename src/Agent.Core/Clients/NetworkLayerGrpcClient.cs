using Agent.Core.Models;
using Common.Grpc;
using NetworkLayer.Grpc.Topology;

namespace Agent.Core.Clients;

public class NetworkLayerGrpcClient : CachedGrpcClient
{
    public async Task<IList<int>> ScaleUp(Uri uri, ICollection<NetworkObjectCreateInfo> newNOs)
    {
        var channel = GetChannel(uri);
        var client = new NetworkTopologyUpdater.NetworkTopologyUpdaterClient(channel);

        var request = new ScaleUpRequest
        {
            NewNetworkObjects = { newNOs.Select(MapNewNetworkObject) }
        };
        
        var response = await client.ScaleUpAsync(request);

        return response.CreatedIds.ToList();
    }

    public async Task<IList<int>> ScaleDown(Uri uri, IEnumerable<int> removeIds)
    {
        var channel = GetChannel(uri);
        var client = new NetworkTopologyUpdater.NetworkTopologyUpdaterClient(channel);

        var request = new ScaleDownRequest { RemoveIds = { removeIds } };

        var response = await client.ScaleDownAsync(request);

        return response.NotFoundIds.ToList();
    }

    private static NewNetworkObject MapNewNetworkObject(NetworkObjectCreateInfo info)
    {
        return new NewNetworkObject();
    }
}
