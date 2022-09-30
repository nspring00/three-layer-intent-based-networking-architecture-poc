using Common.Services;
using NetworkLayer.API.Models;

namespace NetworkLayer.API.Repositories;

public class SimulationNetworkObjectRepository : INetworkObjectRepository
{
    private readonly ILogger<SimulationNetworkObjectRepository> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly SimulationDataSet _dataset;
    private readonly List<(int, DateTime)> _nos;

    private int _newId;

    public SimulationNetworkObjectRepository(ILogger<SimulationNetworkObjectRepository> logger,
        IDateTimeProvider dateTimeProvider, string dataSetPath, int initialCount)
    {
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
        _dataset = new SimulationDataSet(logger, dataSetPath);

        var now = _dateTimeProvider.Now;
        _nos = Enumerable.Range(1, initialCount + 1)
            .Select(x => (x, now))
            .ToList();
        _newId = initialCount + 1;
        _logger.LogInformation("Initialized with {Count} objects", initialCount);
    }

    private class SimulationDataSet
    {
        private readonly string[][] _dataset;
        private int _i = 1;

        public SimulationDataSet(ILogger logger, string datasetPath)
        {
            _dataset = File.ReadAllLines(datasetPath)
                .Select(x => x.Split(";".ToArray()))
                .ToArray();

            logger.LogInformation("Loaded {DatasetSize} lines from dataset", _dataset.Length);
        }

        public DataSetLine GetLine()
        {
            var line = _dataset[_i++];
            if (_i >= _dataset.Length)
            {
                // TODO log??
                _i = 1;
            }

            return new DataSetLine(
                int.Parse(line[0]),
                float.Parse(line[1]),
                float.Parse(line[2]),
                float.Parse(line[3])
            );
        }
    }

    private record DataSetLine(int Id, float CpuWorkload, float MemoryWorkload, float AverageAvailability);

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
                Application = string.Empty,
                CreatedAt = x.Item2,
                Ip = string.Empty,
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
