using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using NetworkLayer.Grpc;

namespace Data.Console.Clients;

public class NlGrpcClient : IDisposable
{
    private readonly ILogger<NlGrpcClient> _logger;

    private readonly Dictionary<Uri, GrpcChannel> _channelCache = new();

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

    private GrpcChannel GetChannel(Uri uri)
    {
        if (_channelCache.ContainsKey(uri))
        {
            return _channelCache[uri];
        } 
        var channel = GrpcChannel.ForAddress(uri);
        _channelCache[uri] = channel;
        return channel;
    }

    public void Dispose()
    {
        // Close all channels
        foreach (var channel in _channelCache.Values)
        {
            channel.ShutdownAsync().Wait();
        }
    }
}
