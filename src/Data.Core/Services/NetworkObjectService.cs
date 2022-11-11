using System.Diagnostics;
using Common.Models;
using Data.Core.Models;
using Data.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace Data.Core.Services;
public class NetworkObjectService : INetworkObjectService
{
    private readonly ILogger<NetworkObjectService> _logger;
    private readonly INetworkObjectRepository _networkObjectRepository;
    private readonly IEfficiencyService _efficiencyService;

    private readonly Dictionary<Region, List<AddedNetworkObject>> _added = new();

    public NetworkObjectService(
        ILogger<NetworkObjectService> logger, 
        INetworkObjectRepository networkObjectRepository,
        IEfficiencyService efficiencyService)
    {
        _logger = logger;
        _networkObjectRepository = networkObjectRepository;
        _efficiencyService = efficiencyService;
    }

    public void Create(NetworkObject networkObject)
    {
        // TODO is _added actually necessary???
        if (!_added.ContainsKey(networkObject.Region))
        {
            _added.Add(networkObject.Region, new List<AddedNetworkObject> { new(networkObject.Id) });
        }
        else
        {
            if (_added[networkObject.Region].Any(x => x.Id == networkObject.Id))
            {
                _logger.LogWarning("NetworkObject already exists: {Id}", networkObject.Id);
                return;
            }
            _added[networkObject.Region].Add(new AddedNetworkObject(networkObject.Id));
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

    public bool Remove(NOId id)
    {
        return _networkObjectRepository.Remove(id);
    }

    public IDictionary<Region, NetworkUpdate> AggregateUpdates(DateTime from, DateTime to)
    {
        Debug.Assert(from < to);
        
        var totalTime = to - from;
        var nos = _networkObjectRepository.GetAll();
        var regions = nos.Keys.Union(_added.Keys).ToList();
        var updates = new Dictionary<Region, NetworkUpdate>(regions.Count);
        
        foreach (var region in regions)
        {
            if (!nos.ContainsKey(region))
            {
                _logger.LogInformation("No NO list found for region {Region}", region.Name);
                continue;
            }

            var deviceInfos = nos[region]
                .Select(x => ComputeAverageInfo(x, from, to, totalTime))
                .Where(x => x is not null)
                .ToList();

            if (deviceInfos.Count == 0)
            {
                _logger.LogInformation("No NOs found for region {Region}", region.Name);
                continue;
            }

            var deviceCount = deviceInfos.Count;
            var avgEfficiency = deviceInfos.Average(x => _efficiencyService.ComputeAvgEfficiency(x!.Utilization));
            var avgAvailability = deviceInfos.Average(x => x!.Availability);

            var update = new NetworkUpdate
            {
                DeviceCount = deviceCount,
                AvgEfficiency = avgEfficiency,
                AvgAvailability = avgAvailability
            };

            _logger.LogInformation(
                "Computed stats for Region {Region}: DeviceCount {DeviceCount}, AvgEfficiency {AvgEfficiency}, AvgAvailability {AvgAvailability}",
                region.Name, deviceCount, avgEfficiency, avgAvailability);

            updates.Add(region, update);
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
                _logger.LogError("Unexpected timestamp before aggregation: {InfoKey}", info.Key);
            }
            
            if (info.Key > end)
            {
                _logger.LogError("Unexpected timestamp in the future {InfoKey}", info.Key);
                continue;
            }

            var elapsed = info.Key - last;
            last = info.Key;
            var weight = (float) (elapsed / totalTime);
            result.Utilization.CpuUtilization += weight * info.Value.Utilization.CpuUtilization;
            result.Utilization.MemoryUtilization += weight * info.Value.Utilization.MemoryUtilization;
            result.Availability += weight * info.Value.Availability;
            //System.Console.WriteLine($"{info.Key} {info.Value.Utilization.CpuUtilization} {info.Value.Utilization.MemoryUtilization} {info.Value.Availability}");
        }

        // Clear all status infos about network objects
        networkObject.Infos.Clear();

        //System.Console.WriteLine($"{networkObject.Id} {result.Utilization.CpuUtilization} {result.Utilization.MemoryUtilization} {result.Availability}");
        _logger.LogInformation("Device {DeviceId}: CPU {Cpu}, Memory {Mem}, Availability {Availability}", networkObject.Id,
            result.Utilization.CpuUtilization, result.Utilization.MemoryUtilization, result.Availability);

        return result;
    }

    public void Reset()
    {
        _added.Clear();
    }

    public bool Exists(NOId id)
    {
        return _networkObjectRepository.Get(id) != null;
    }
}
