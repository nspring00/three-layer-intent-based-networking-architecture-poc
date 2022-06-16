using Grpc.Net.Client;

namespace Data.Console.Clients;

public class CachedGrpcClient : IDisposable
{
    private readonly Dictionary<Uri, GrpcChannel> _channelCache = new();

    protected GrpcChannel GetChannel(Uri uri)
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
