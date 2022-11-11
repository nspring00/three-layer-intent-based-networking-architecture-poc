using System.Collections.Concurrent;
using Common.Models;
using Common.Services;
using Knowledge.API.Models;

namespace Knowledge.API.Repository;

public class CachedWorkloadRepository : IWorkloadRepository
{
    private readonly ILogger<CachedWorkloadRepository> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ConcurrentDictionary<Region, List<WorkloadInfo>> _workloadInfos = new();
    private readonly ConcurrentDictionary<Region, IdGenerator> _regionIdGenerators = new();

    public CachedWorkloadRepository(ILogger<CachedWorkloadRepository> logger, IDateTimeProvider dateTimeProvider)
    {
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
    }

    public void Add(Region region, WorkloadInfo workloadInfo)
    {
        workloadInfo.Timestamp = _dateTimeProvider.Now;
        workloadInfo.Id = GetIdGenerator(region).NextId();
        _logger.LogInformation("Adding workload info {InfoId} for region {Region}", workloadInfo.Id, region.Name);
        GetWorkloadList(region).Insert(0, workloadInfo);
    }

    public IList<WorkloadInfo> GetForRegion(Region region)
    {
        _workloadInfos.TryGetValue(region, out var workloadInfos);
        return workloadInfos ?? new List<WorkloadInfo>();
    }    
    
    public IList<WorkloadInfo> GetForRegion(Region region, int count)
    {
        _workloadInfos.TryGetValue(region, out var workloadInfos);
        return workloadInfos?.Take(count).ToList() ?? new List<WorkloadInfo>();
    }

    public WorkloadInfo? GetLatest(Region region)
    {
        _workloadInfos.TryGetValue(region, out var workloadInfos);
        return workloadInfos?.FirstOrDefault();
    }

    private IdGenerator GetIdGenerator(Region region)
    {
        return _regionIdGenerators.GetOrAdd(region, new IdGenerator());
    }

    private List<WorkloadInfo> GetWorkloadList(Region region)
    {
        return _workloadInfos.GetOrAdd(region, new List<WorkloadInfo>());
    }

    //public NetworkDevice? Get(Region region, int id)
    //{
    //    return _devices.FirstOrDefault(d => d.Id == id && d.Region == region);
    //}

    //public IList<NetworkDevice> GetForRegion(Region region)
    //{
    //    return _devices.Where(i => i.Region == region).ToList();
    //}

    private class IdGenerator
    {
        private int _nextId = 1;
        public int NextId() => _nextId++;
    }
}
