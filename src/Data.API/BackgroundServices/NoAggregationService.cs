using Common.Services;
using Data.API.Services;
using Data.Core.Services;

namespace Data.API.BackgroundServices;

public class NoAggregationService : BackgroundService
{
    private readonly ILogger<NoAggregationService> _logger;
    private readonly INetworkLayerService _networkLayerService;
    private readonly INetworkObjectService _networkObjectService;
    private readonly IKnowledgeService _knowledgeService;
    private readonly INlManagerService _nlManagerService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public NoAggregationService(
        ILogger<NoAggregationService> logger,
        INetworkLayerService networkLayerService,
        INetworkObjectService networkObjectService,
        IKnowledgeService knowledgeService,
        INlManagerService nlManagerService,
        IDateTimeProvider dateTimeProvider)
    {
        _logger = logger;
        _networkLayerService = networkLayerService;
        _networkObjectService = networkObjectService;
        _knowledgeService = knowledgeService;
        _nlManagerService = nlManagerService;
        _dateTimeProvider = dateTimeProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var startTime = _dateTimeProvider.Now;

        while (!ct.IsCancellationRequested)
        {
            for (var i = 0; i < 4; i++)
            {
                ct.WaitHandle.WaitOne(2500);
                if (ct.IsCancellationRequested)
                {
                    return;
                }

                await FetchAllUpdates();
            }

            var endTime = _dateTimeProvider.Now;
            var updates = _networkObjectService.AggregateUpdates(startTime, endTime);
            _networkObjectService.Reset();
            await _knowledgeService.UpdateKnowledgeNOs(endTime, updates);
            startTime = endTime;
        }
    }

    private async Task FetchAllUpdates()
    {
        var nlInfos = _nlManagerService.GetAll();
        _logger.LogInformation($"Fetching updates for {nlInfos.Count} network layers");

        foreach (var nlInfo in nlInfos)
        {
            _logger.LogInformation("Fetching updates for {NlId}: {NlName} ({NlRegion}) {NlUri}", nlInfo.Id, nlInfo.Name,
                nlInfo.Region.Name, nlInfo.Uri);
            try
            {
                await _networkLayerService.FetchAllUpdates(nlInfo.Id, nlInfo.Region, nlInfo.Uri);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to fetch updates for {NlId}", nlInfo.Id);
            }
        }
    }
}
