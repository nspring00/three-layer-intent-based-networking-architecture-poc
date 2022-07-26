using Agent.Core.Clients;
using Agent.Core.Models;

namespace Agent.Core.Services;

public class GrpcNetworkLayerService : INetworkLayerService
{
    private readonly NetworkLayerGrpcClient _client;

    public GrpcNetworkLayerService(NetworkLayerGrpcClient client)
    {
        _client = client;
    }

    public Task<IList<int>> CreateNOsAsync(Uri uri, ICollection<NetworkObjectCreateInfo> newNOs)
    {
        return _client.ScaleUp(uri, newNOs);
    }

    public Task<IList<int>> DeleteNOsAsync(Uri uri, ICollection<int> removeIds)
    {
        return _client.ScaleDown(uri, removeIds);
    }
}
