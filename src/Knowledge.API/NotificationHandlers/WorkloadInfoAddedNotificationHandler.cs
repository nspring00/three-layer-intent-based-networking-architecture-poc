using System.Globalization;
using Common.Web.AspNetCore;
using Knowledge.API.Models;
using Knowledge.API.Repository;
using Knowledge.API.Services;
using MediatR;

namespace Knowledge.API.NotificationHandlers;

public class WorkloadInfoAddedNotificationHandler : INotificationHandler<WorkloadInfoAddedNotification>
{
    private readonly ILogger<WorkloadInfoAddedNotificationHandler> _logger;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IReasoningService _reasoningService;
    private readonly IAgentService _agentService;
    private readonly IWorkloadRepository _workloadRepository;

    private const string OutputFileName = "output.csv";
    private static bool _isInitialized;
    private static int _outputId;

    public WorkloadInfoAddedNotificationHandler(
        ILogger<WorkloadInfoAddedNotificationHandler> logger,
        IHostEnvironment hostEnvironment,
        IReasoningService reasoningService,
        IAgentService agentService,
        IWorkloadRepository workloadRepository)
    {
        _logger = logger;
        _hostEnvironment = hostEnvironment;
        _reasoningService = reasoningService;
        _agentService = agentService;
        _workloadRepository = workloadRepository;

        if (!_hostEnvironment.IsDockerSimulation() || _isInitialized)
        {
            return;
        }

        _isInitialized = true;
        InitSimulation();
    }

    private static void InitSimulation()
    {
        File.WriteAllText(OutputFileName, "Id;DeviceCount;EffTrend;AvailTrend\n");
        // Knowledge is first notified on 4th check of the NL
        for (var i = 0; i < 4; i++)
        {
            File.AppendAllText(OutputFileName, $"{Interlocked.Increment(ref _outputId)};;;\n");
        }
    }

    public async Task Handle(WorkloadInfoAddedNotification notification, CancellationToken cancellationToken)
    {
        if (notification.Regions.Count == 0)
        {
            _logger.LogWarning("No regions found in notification");
            return;
        }
        
        if (_hostEnvironment.IsDockerSimulation())
        {
            _logger.LogInformation("Outputting simulation data for region {Region}", notification.Regions.First());
            
            var kpis = new[]
            {
                KeyPerformanceIndicator.Efficiency,
                KeyPerformanceIndicator.Availability
            };
            var infos = _workloadRepository.GetForRegion(notification.Regions.First(),
                _reasoningService.MaxInfosForReasoning);
            var trends = _reasoningService.GenerateKpiTrends(infos, kpis);

            var culture = CultureInfo.GetCultureInfo("de");
            // do this 4 times because data collects 4 times per update
            for (var i = 0; i < 4; i++)
            {
                await File.AppendAllTextAsync(OutputFileName, $"{Interlocked.Increment(ref _outputId)};{infos.MaxBy(x => x.Id)!.DeviceCount};" +
                                                              $"{trends[KeyPerformanceIndicator.Efficiency].ToString(culture)};" +
                                                              $"{trends[KeyPerformanceIndicator.Availability].ToString(culture)}\n", cancellationToken);
            }
        }
        
        _logger.LogInformation("Handling WorkloadInfoAddedNotification for regions {Regions}", notification.Regions);

        var allAgents = _reasoningService.QuickReasoningForRegions(notification.Regions);
        
        _logger.LogInformation("Found {AgentCount} agents for regions {Regions}", allAgents.Count, notification.Regions);

        var agentsToNotify = allAgents
            .Where(x => x.Value)
            .Select(x => x.Key)
            .ToList();

        if (agentsToNotify.Count == 0)
        {
            _logger.LogInformation("No agents need to be notified");
            return;
        }

        _logger.LogInformation("Notifying agents for {RegionCount} out of {MaxRegionCount} regions: {Region}",
            agentsToNotify.Count, allAgents.Count, agentsToNotify.Select(x => x.Name));

        await _agentService.NotifyAgents(agentsToNotify);
    }
}
