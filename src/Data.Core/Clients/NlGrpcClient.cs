using Common.Grpc;
using Microsoft.Extensions.Logging;
using NetworkLayer.Grpc.NetworkObjects;

namespace Data.Core.Clients;

public class NlGrpcClient : CachedGrpcClient
{
    public async Task<NetworkObjectUpdateResponse> FetchUpdates(Uri uri)
    {
        var channel = GetChannel(uri);
        var client = new NetworkObjectUpdater.NetworkObjectUpdaterClient(channel);
        return await client.GetUpdateAsync(new NetworkObjectUpdateRequest());
    }
}
