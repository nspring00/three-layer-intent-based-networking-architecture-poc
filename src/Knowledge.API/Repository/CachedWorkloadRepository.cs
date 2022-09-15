using System.Collections.Concurrent;
using Common.Models;
using Common.Services;
using Knowledge.API.Models;

namespace Knowledge.API.Repository;

public class CachedWorkloadRepository : IWorkloadRepository
{
    private readonly ILogger<CachedWorkloadRepository> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    //private readonly List<NetworkDevice> _devices = new();
    private readonly ConcurrentDictionary<Region, List<WorkloadInfo>> _workloadInfos = new();
    private readonly ConcurrentDictionary<Region, IdGenerator> _regionIdGenerators = new();

    private const bool InsertTestData = true;

    // TODO remove after testing
    public CachedWorkloadRepository(ILogger<CachedWorkloadRepository> logger, IDateTimeProvider dateTimeProvider)
    {
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;

        if (InsertTestData)
        {
            _workloadInfos[new Region("Vienna")] = new List<WorkloadInfo>()
            {
                new()
                {
                    Id = 1,
                    Timestamp = _dateTimeProvider.Now,
                    DeviceCount = 5,
                    AvgEfficiency = 0.9f,
                    AvgAvailability = 1f
                },
                new()
                {
                    Id = 2,
                    Timestamp = _dateTimeProvider.Now,
                    DeviceCount = 5,
                    AvgEfficiency = 0.7f,
                    AvgAvailability = 1f
                }
            };

            //_devices.Add(new NetworkDevice(
            //    1, new Region("Vienna"), "WorkerApp1",
            //    new Utilization { CpuUtilization = 0.9f, MemoryUtilization = 0.9f }));
            //_devices.Add(new NetworkDevice(
            //    2, new Region("Vienna"), "WorkerApp1",
            //    new Utilization { CpuUtilization = 0.9f, MemoryUtilization = 0.9f }));
            //_devices.Add(new NetworkDevice(
            //    3, new Region("Vienna"), "WorkerApp1",
            //    new Utilization { CpuUtilization = 0.9f, MemoryUtilization = 0.9f }));
            //_devices.Add(new NetworkDevice(
            //    4, new Region("Vienna"), "WorkerApp1",
            //    new Utilization { CpuUtilization = 0.9f, MemoryUtilization = 0.9f }));
            //_devices.Add(new NetworkDevice(
            //    5, new Region("Vienna"), "WorkerApp1",
            //    new Utilization { CpuUtilization = 0.9f, MemoryUtilization = 0.9f }));
            //_devices.Add(new NetworkDevice(
            //    1, new Region("Linz"), "WorkerApp1",
            //    new Utilization { CpuUtilization = 0.7f, MemoryUtilization = 0.7f }));
            //_devices.Add(new NetworkDevice(
            //    2, new Region("Linz"), "WorkerApp1",
            //    new Utilization { CpuUtilization = 0.7f, MemoryUtilization = 0.7f }));
            //_devices.Add(new NetworkDevice(
            //    3, new Region("Linz"), "WorkerApp1",
            //    new Utilization { CpuUtilization = 0.7f, MemoryUtilization = 0.7f }));
            //_devices.Add(new NetworkDevice(
            //    4, new Region("Linz"), "WorkerApp1",
            //    new Utilization { CpuUtilization = 0.7f, MemoryUtilization = 0.7f }));
            //_devices.Add(new NetworkDevice(
            //    5, new Region("Linz"), "WorkerApp1",
            //    new Utilization { CpuUtilization = 0.7f, MemoryUtilization = 0.7f }));
        }
    }

    //public void Add(NetworkDevice device)
    //{
    //    _logger.LogInformation($"Adding device {device}");
    //    if (_devices.Any(d => d.Id == device.Id && d.Region == device.Region))
    //    {
    //        _logger.LogWarning($"Device {device} already exists");
    //        return;
    //    }

    //    _devices.Add(device);
    //}

    //public bool Remove(Region region, int id)
    //{
    //    _logger.LogInformation($"Removing device {region} {id}");
    //    var removed = _devices.RemoveAll(d => d.Id == id && d.Region == region);
    //    return removed > 0;
    //}

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
