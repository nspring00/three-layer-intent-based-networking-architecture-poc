using Common.Services;
using Data.API.Options;
using Data.API.Services;
using Data.Core.Services;
using Microsoft.Extensions.Options;

namespace Data.API.BackgroundServices;

public class NoAggregationService : BackgroundService
{
    private readonly ILogger<NoAggregationService> _logger;
    private readonly INetworkLayerService _networkLayerService;
    private readonly INetworkObjectService _networkObjectService;
    private readonly IKnowledgeService _knowledgeService;
    private readonly INlManagerService _nlManagerService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly AggregationConfig _aggregationConfig;

    public NoAggregationService(
        ILogger<NoAggregationService> logger,
        INetworkLayerService networkLayerService,
        INetworkObjectService networkObjectService,
        IKnowledgeService knowledgeService,
        INlManagerService nlManagerService,
        IDateTimeProvider dateTimeProvider,
        IOptions<AggregationConfig> aggregationConfig)
    {
        _logger = logger;
        _networkLayerService = networkLayerService;
        _networkObjectService = networkObjectService;
        _knowledgeService = knowledgeService;
        _nlManagerService = nlManagerService;
        _dateTimeProvider = dateTimeProvider;
        _aggregationConfig = aggregationConfig.Value;
        
        _logger.LogInformation(
            "AggregationService configured with UpdateInterval: {UpdateInterval} and AfterKnowledgeUpdateTimeout: {AfterKnowledgeUpdateTimeout}", 
            _aggregationConfig.UpdateInterval, _aggregationConfig.AfterKnowledgeUpdateTimeout);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var startTime = _dateTimeProvider.Now;

        while (!ct.IsCancellationRequested)
        {
            for (var i = 0; i < 4; i++)
            {
                ct.WaitHandle.WaitOne(_aggregationConfig.UpdateInterval);
                if (ct.IsCancellationRequested)
                {
                    return;
                }

                await FetchAllUpdates();
            }

            var endTime = _dateTimeProvider.Now;
            var updates = _networkObjectService.AggregateUpdates(startTime, endTime);

            if (updates.Count == 0)
            {
                // No updates, so we can skip the rest of the loop
                continue;
            }
            
            _networkObjectService.Reset();
            await _knowledgeService.UpdateKnowledgeNOs(endTime, updates);
        
            ct.WaitHandle.WaitOne(_aggregationConfig.AfterKnowledgeUpdateTimeout);

            startTime = endTime;
        }
    }

    private async Task FetchAllUpdates()
    {
        var nlInfos = _nlManagerService.GetAll();
        _logger.LogInformation("Fetching updates for {NlInfosCount} network layers", nlInfos.Count);

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
