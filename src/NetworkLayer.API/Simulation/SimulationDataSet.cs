using System.Globalization;
using Microsoft.Extensions.Options;
using NetworkLayer.API.Options;

namespace NetworkLayer.API.Simulation;

public class SimulationDataSet
{
    private readonly ILogger<SimulationDataSet> _logger;
    private readonly string[][] _dataset;
    private int _i = 1;

    public SimulationDataSet(ILogger<SimulationDataSet> logger, IOptions<SimulationConfig> config)
    {
        _logger = logger;
        
        _logger.LogInformation("Loading dataset from {DataSetPath}", config.Value.DataSetPath);
        
        _dataset = File.ReadAllLines(config.Value.DataSetPath)
            .Where(x => !string.IsNullOrEmpty(x))
            .Select(x => x.Split(";".ToArray()))
            .ToArray();

        logger.LogInformation("Loaded {DatasetSize} lines from dataset", _dataset.Length);
    }

    public DataSetLine GetLine()
    {
        var line = _dataset[_i++];
        if (_i >= _dataset.Length)
        {
            _logger.LogInformation("Dataset is finished. Restarting from the beginning");
            _i = 1;
        }

        _logger.LogInformation("Returning line {Line}", int.Parse(line[0]));
        return new DataSetLine(
            int.Parse(line[0]),
            float.Parse(line[1], CultureInfo.InvariantCulture),
            float.Parse(line[2], CultureInfo.InvariantCulture),
            float.Parse(line[3], CultureInfo.InvariantCulture)
        );
    }
}
