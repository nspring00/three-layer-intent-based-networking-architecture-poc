using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using NetworkLayer.Grpc;

namespace Data.Console.Clients;

public class NlGrpcClient : CachedGrpcClient
{
    private readonly ILogger<NlGrpcClient> _logger;

    public NlGrpcClient(ILogger<NlGrpcClient> logger)
    {
        _logger = logger;
    }

    public async Task<NetworkObjectUpdateResponse> FetchUpdates(Uri uri)
    {
        var channel = GetChannel(uri);
        var client = new NetworkObjectUpdater.NetworkObjectUpdaterClient(channel);
        return await client.GetUpdateAsync(new NetworkObjectUpdateRequest());
    }
}
