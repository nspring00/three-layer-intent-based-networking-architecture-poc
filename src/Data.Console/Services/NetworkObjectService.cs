using System.Diagnostics;
using Data.Console.Models;
using Data.Console.Repositories;
using Microsoft.Extensions.Logging;

namespace Data.Console.Services;
public class NetworkObjectService : INetworkObjectService
{
    private readonly ILogger<NetworkObjectService> _logger;
    private readonly INetworkObjectRepository _networkObjectRepository;

    private readonly Dictionary<Region, List<int>> _added = new();
    private readonly Dictionary<Region, List<int>> _removed = new();

    public NetworkObjectService(
        ILogger<NetworkObjectService> logger, 
        INetworkObjectRepository networkObjectRepository)
    {
        _logger = logger;
        _networkObjectRepository = networkObjectRepository;
    }

    public void Create(NetworkObject networkObject)
    {
        if (!_added.ContainsKey(networkObject.Region))
        {
            _added.Add(networkObject.Region, new List<int>(networkObject.Id));
        }
        else
        {
            _added[networkObject.Region].Add(networkObject.Id);
        }
        
        _networkObjectRepository.Create(networkObject);
    }
    
    public void AddInfo(NOId id, DateTime updateTime, NetworkObjectInfo info)
    {
        var no = _networkObjectRepository.Get(id);
        if (no is null)
        {
            _logger.LogWarning("NetworkObject not found: {Id}", id);
            return;
        }

        no.Infos.Add(updateTime, info);
    }

    public void Remove(NOId id)
    {
        if (!_removed.ContainsKey(id.Region))
        {
            _removed.Add(id.Region, new List<int>(id.Id));
        }
        else
        {
            _removed[id.Region].Add(id.Id);
        }

        _networkObjectRepository.Remove(id);
    }

    public IDictionary<Region, NetworkUpdate> AggregateUpdates(DateTime from, DateTime to)
    {
        Debug.Assert(from < to);
        
        var totalTime = to - from;
        var nos = _networkObjectRepository.GetAll();
        var regions = nos.Keys.Union(_added.Keys).Union(_removed.Keys).ToList();
        var updates = new Dictionary<Region, NetworkUpdate>(regions.Count);
        
        foreach (var region in regions)
        {
            var update = new NetworkUpdate
            {
                Timestamp = to
            };
            updates.Add(region, update);

            if (nos.ContainsKey(region))
            {
                foreach (var networkObject in nos[region])
                {
                    var info = ComputeAverageInfo(networkObject, from, to, totalTime);
                    if (info is null) continue;

                    update.Updates.Add(networkObject.Id, info);

                }
            }

            if (_added.ContainsKey(region))
            {
                var added = _added[region];
                if (added.Count == 0)
                {
                    continue;
                }

                update.Added.AddRange(added);
            }

            if (_removed.ContainsKey(region))
            {
                var removed = _removed[region];
                if (removed.Count == 0)
                {
                    continue;
                }

                update.Removed.AddRange(removed);
            }
        }

        return updates;
    }

    private NetworkObjectInfo? ComputeAverageInfo(NetworkObject networkObject, DateTime start, DateTime end, TimeSpan totalTime)
    {
        if (networkObject.Infos.Count == 0)
        {
            return null;
        }

        if (networkObject.Created > start)
        {
            start = networkObject.Created;
            totalTime = end - start;
            Debug.Assert(start < end);
        }

        // Timestamps are in-order
        var last = start;
        var result = new NetworkObjectInfo();
        if (!networkObject.Infos.ContainsKey(end))
        {
            networkObject.Infos.Add(end, networkObject.Infos.Last().Value);
        }
        
        foreach (var info in networkObject.Infos)
        {
            if (info.Key < start)
            {
                _logger.LogError($"Unexpected timestamp before aggregation: {info.Key}");
            }
            
            if (info.Key > end)
            {
                _logger.LogError($"Unexpected timestamp in the future {info.Key}");
                continue;
            }

            var elapsed = info.Key - last;
            last = info.Key;
            var weight = (float) (elapsed / totalTime);
            result.Utilization.CpuUtilization += weight * info.Value.Utilization.CpuUtilization;
            result.Utilization.MemoryUtilization += weight * info.Value.Utilization.MemoryUtilization;
            result.Availability += weight * info.Value.Availability;
            System.Console.WriteLine($"{info.Key} {info.Value.Utilization.CpuUtilization} {info.Value.Utilization.MemoryUtilization} {info.Value.Availability}");
        }

        // Clear all status infos about network objects
        networkObject.Infos.Clear();

        System.Console.WriteLine($"{networkObject.Id} {result.Utilization.CpuUtilization} {result.Utilization.MemoryUtilization} {result.Availability}");
        return result;
    }

    public void Reset()
    {
        _added.Clear();
        _removed.Clear();
    }
}
