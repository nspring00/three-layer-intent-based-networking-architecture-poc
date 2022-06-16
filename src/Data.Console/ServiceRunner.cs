using Data.Console.Models;
using Data.Console.Services;
using Microsoft.Extensions.Logging;

namespace Data.Console;

public class ServiceRunner
{
    private readonly ILogger<ServiceRunner> _logger;
    private readonly INetworkLayerService _networkLayerService;
    private readonly INetworkObjectService _networkObjectService;
    private readonly IKnowledgeService _knowledgeService;

    private readonly IDictionary<Region, IList<Uri>> _uris = new Dictionary<Region, IList<Uri>>
    {
        {
            new Region("Vienna"), new List<Uri>
            {
                new("https://localhost:7071")
            }
        }
    };

    public ServiceRunner(
        ILogger<ServiceRunner> logger,
        INetworkLayerService networkLayerService,
        INetworkObjectService networkObjectService,
        IKnowledgeService knowledgeService)
    {
        _logger = logger;
        _networkLayerService = networkLayerService;
        _networkObjectService = networkObjectService;
        _knowledgeService = knowledgeService;
    }

    public async Task Run()
    {
        // TODO do this (maybe multiple times) first
        var startTime = DateTime.UtcNow;

        while (true)
        {
            for (var i = 0; i < 4; i++)
            {
                Thread.Sleep(2500);
                await FetchAllUpdates();

            }
            var endTime = DateTime.UtcNow;
            var updates = _networkObjectService.AggregateUpdates(startTime, endTime);
            _networkObjectService.Reset();
            await _knowledgeService.UpdateKnowledgeNOs(updates);
            startTime = endTime;
        }
    }
    
    private async Task FetchAllUpdates()
    {
        foreach (var (region, uris) in _uris)
        {
            foreach (var uri in uris)
            {
                await _networkLayerService.FetchAllUpdates(region, uri);
            }
        }
    }
}
