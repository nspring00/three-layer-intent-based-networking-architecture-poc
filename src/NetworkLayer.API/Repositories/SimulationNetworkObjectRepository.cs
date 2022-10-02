using Common.Services;
using Microsoft.Extensions.Options;
using NetworkLayer.API.Models;
using NetworkLayer.API.Options;
using NetworkLayer.API.Simulation;

namespace NetworkLayer.API.Repositories
{
    public class SimulationNetworkObjectRepository : INetworkObjectRepository
    {
        private readonly ILogger<SimulationNetworkObjectRepository> _logger;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly SimulationDataSet _dataset;
        private readonly List<(int, DateTime)> _nos;

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
            _newId = config.Value.InitialCount + 1;
            _logger.LogInformation("Initialized with {Count} objects", config.Value.InitialCount);
        }

        public IList<NetworkObject> GetAll()
        {
            var (id, cpuWorkload, memoryWorkload, avgAvail) = _dataset.GetLine();
            _logger.LogInformation("Loaded dataset line {Id} with {CpuWorkload} {MemoryWorkload} {AvgAvail}", id,
                cpuWorkload, memoryWorkload, avgAvail);

            var avgCpu = cpuWorkload / _nos.Count;
            var avgMem = memoryWorkload / _nos.Count;

            return _nos.Select(x => new NetworkObject
                {
                    Id = x.Item1,
                    CreatedAt = x.Item2,
                    Utilization = new Utilization
                    {
                        CpuUtilization = avgCpu, // TODO jitter values
                        MemoryUtilization = avgMem,
                    },
                    Availability = avgAvail
                })
                .ToList();
        }

        public void Create(NetworkObject networkObject)
        {
            networkObject.Id = _newId++;
            networkObject.CreatedAt = _dateTimeProvider.Now;
            _nos.Add((networkObject.Id, networkObject.CreatedAt));
        }

        public bool Delete(int id)
        {
            return _nos.RemoveAll(x => x.Item1 == id) > 0;
        }
    }
}
