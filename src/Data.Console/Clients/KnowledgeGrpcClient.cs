﻿using Data.Console.Models;
using Knowledge.Grpc.NetworkInfoUpdate;
using Microsoft.Extensions.Logging;
using Utilization = Knowledge.Grpc.NetworkInfoUpdate.Utilization;

namespace Data.Console.Clients;

public class KnowledgeGrpcClient : CachedGrpcClient
{
    private readonly ILogger<KnowledgeGrpcClient> _logger;

    public KnowledgeGrpcClient(ILogger<KnowledgeGrpcClient> logger)
    {
        _logger = logger;
    }

    public async Task UpdateKnowledge(Uri uri, IDictionary<Region, NetworkUpdate> updates)
    {
        var channel = GetChannel(uri);
        var client = new NetworkInfoUpdater.NetworkInfoUpdaterClient(channel);

        var request = CreateRequest(updates);
        await client.UpdateAsync(request);
    }

    private static NetworkInfoUpdateRequest CreateRequest(IDictionary<Region, NetworkUpdate> updates)
    {
        var request = new NetworkInfoUpdateRequest();
        request.RegionUpdates.AddRange(updates.Select(x =>
        {
            return new RegionUpdate
            {
                RegionName = x.Key.Name,
                TopologyUpdates =
                {
                    x.Value.Added.Select(id => new TopologyUpdate
                    {
                        NetworkObjectId = id,
                        UpdateType = TopologyUpdate.Types.NetworkObjectUpdateType.Add
                    }),
                    x.Value.Removed.Select(id => new TopologyUpdate
                    {
                        NetworkObjectId = id,
                        UpdateType = TopologyUpdate.Types.NetworkObjectUpdateType.Remove
                    })
                },
                WorkloadUpdates =
                {
                    x.Value.Updates.Select(update => new WorkloadUpdate
                    {
                        NetworkObjectId = update.Key,
                        Utilization = new Utilization
                        {
                            CpuUtilization = update.Value.Utilization.CpuUtilization,
                            MemoryUtilization = update.Value.Utilization.MemoryUtilization
                        },
                        Availability = update.Value.Availability
                    })
                }
            };
        }));

        return request;
    }
}
