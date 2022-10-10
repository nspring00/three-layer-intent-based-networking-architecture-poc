using System.Globalization;
using Common.Services;
using Microsoft.Extensions.Options;
using NetworkLayer.API.Models;
using NetworkLayer.API.Options;
using NetworkLayer.API.Simulation;

namespace NetworkLayer.API.Repositories
{
    public class SimulationNetworkObjectRepository : INetworkObjectRepository
    {
        private const string OutputFileName = "output.csv";
        
        private readonly ILogger<SimulationNetworkObjectRepository> _logger;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly SimulationDataSet _dataset;
        private readonly List<(int, DateTime)> _nos;

        private readonly List<NetworkObject> _recentlyCreated;
        private readonly List<(NetworkObject, DateTime)> _recentlyRemoved = new();

        private int _newId;

        public SimulationNetworkObjectRepository(ILogger<SimulationNetworkObjectRepository> logger,
            IDateTimeProvider dateTimeProvider, SimulationDataSet dataset, IOptions<SimulationConfig> config)
        {
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
            _dataset = dataset;

            var now = _dateTimeProvider.Now;
            _nos = Enumerable.Range(1, config.Value.InitialCount)
                .Select(x => (x, now))
                .ToList();
            
            _recentlyCreated = _nos.Select(x => CreateFromSimulation(x)).ToList();
            
            _newId = config.Value.InitialCount + 1;
            _logger.LogInformation("Initialized with {Count} objects", config.Value.InitialCount);
            
            File.WriteAllText(OutputFileName, "Id;DeviceCount;CpuWorkload;CpuEfficiency;MemoryWorkload;MemoryEfficiency;AverageAvailability\n");
        }

        public IList<NetworkObject> GetAll()
        {
            var (id, cpuWorkload, memoryWorkload, avgAvail) = _dataset.GetLine();
            _logger.LogInformation("Loaded dataset line {Id} with {CpuWorkload} {MemoryWorkload} {AvgAvail}", id,
                cpuWorkload, memoryWorkload, avgAvail);

            var avgCpu = Math.Clamp(cpuWorkload / _nos.Count, 0, 1);
            var avgMem = Math.Clamp(memoryWorkload / _nos.Count, 0, 1);
            
            // Output for graph
            var culture = CultureInfo.GetCultureInfo("de");
            File.AppendAllText("output.csv", $"{id};{_nos.Count};{cpuWorkload.ToString(culture)};" +
                                             $"{avgCpu.ToString(culture)};{memoryWorkload.ToString(culture)};" +
                                             $"{avgMem.ToString(culture)};{avgAvail.ToString(culture)}\n");

            return _nos.Select(x => CreateFromSimulation(x, avgCpu, avgMem, avgAvail)).ToList();
        }

        public IList<NetworkObject> GetCreated()
        {
            return new List<NetworkObject>(_recentlyCreated);
        }

        public IList<(NetworkObject, DateTime)> GetRemoved()
        {
            return new List<(NetworkObject, DateTime)>(_recentlyRemoved);
        }

        public void Create(NetworkObject networkObject)
        {
            networkObject.Id = _newId++;
            networkObject.CreatedAt = _dateTimeProvider.Now;
            _nos.Add((networkObject.Id, networkObject.CreatedAt));
            _recentlyCreated.Add(networkObject);
        }

        public bool Delete(int id)
        {
            var toRemove = _nos.Where(x => x.Item1 == id).ToList();
            if (toRemove.Count == 0)
            {
                return false;
            }

            var now = _dateTimeProvider.Now;
            _recentlyRemoved.AddRange(toRemove.Select(x => (new NetworkObject
            {
                Id = x.Item1, CreatedAt = x.Item2
            }, now)));

            _nos.RemoveAll(x => x.Item1 == id);
            return true;
        }

        public void ResetCreateDelete()
        {
            _recentlyCreated.Clear();
            _recentlyRemoved.Clear();
        }

        private static NetworkObject CreateFromSimulation((int, DateTime) no, float? avgCpu = null, float? avgMem = null,
            float? avgAvail = null)
        {
            var (id, createdAt) = no;

            if (avgCpu is null || avgMem is null || avgAvail is null)
            {
                return new NetworkObject
                {
                    Id = id, CreatedAt = createdAt
                };
            }
            
            return new NetworkObject
            {
                Id = id,
                CreatedAt = createdAt,
                Utilization = new Utilization
                {
                    CpuUtilization = avgCpu.Value, // TODO jitter values
                    MemoryUtilization = avgMem.Value,
                },
                Availability = avgAvail.Value
            };
        }
    }
}
