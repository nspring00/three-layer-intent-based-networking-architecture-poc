using Common.Grpc;
using Common.Models;
using Data.Console.Models;
using Google.Protobuf.WellKnownTypes;
using Knowledge.Grpc.NetworkInfoUpdate;
using Microsoft.Extensions.Logging;

namespace Data.Console.Clients;

public class KnowledgeGrpcClient : CachedGrpcClient
{
    private readonly ILogger<KnowledgeGrpcClient> _logger;

    public KnowledgeGrpcClient(ILogger<KnowledgeGrpcClient> logger)
    {
        _logger = logger;
    }

    public async Task UpdateKnowledge(Uri uri, DateTime timestamp, IDictionary<Region, NetworkUpdate> updates)
    {
        var channel = GetChannel(uri);
        var client = new NetworkInfoUpdater.NetworkInfoUpdaterClient(channel);

        var request = CreateRequest(timestamp, updates);
        await client.UpdateAsync(request);
    }

    private static NetworkInfoUpdateRequest CreateRequest(DateTime timestamp, IDictionary<Region, NetworkUpdate> updates)
    {
        var request = new NetworkInfoUpdateRequest
        {
            Timestamp = timestamp.ToTimestamp()
        };
        request.RegionUpdates.AddRange(updates.Select(x => new RegionUpdate
        {
            RegionName = x.Key.Name,
            WorkloadInfo = new WorkloadInfo
            {
                DeviceCount = x.Value.DeviceCount,
                AvgEfficiency = x.Value.AvgEfficiency,
                AvgAvailability = x.Value.AvgAvailability
            }
        }));

        return request;
    }
}
