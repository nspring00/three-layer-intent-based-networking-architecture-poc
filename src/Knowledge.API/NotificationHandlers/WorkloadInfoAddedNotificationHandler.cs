using Knowledge.API.Models;
using Knowledge.API.Services;
using MediatR;

namespace Knowledge.API.NotificationHandlers;

public class WorkloadInfoAddedNotificationHandler : INotificationHandler<WorkloadInfoAddedNotification>
{
    private readonly ILogger<WorkloadInfoAddedNotificationHandler> _logger;
    private readonly IWorkloadAnalysisService _workloadAnalysisService;
    private readonly IAgentService _agentService;

    public WorkloadInfoAddedNotificationHandler(
        ILogger<WorkloadInfoAddedNotificationHandler> logger,
        IWorkloadAnalysisService workloadAnalysisService,
        IAgentService agentService)
    {
        _logger = logger;
        _workloadAnalysisService = workloadAnalysisService;
        _agentService = agentService;
    }

    public Task Handle(WorkloadInfoAddedNotification notification, CancellationToken cancellationToken)
    {
        var allAgents= _workloadAnalysisService.CheckIfAgentsShouldBeNotified(notification.Regions);

        var agentsToNotify = allAgents
            .Where(x => x.Value)
            .Select(x => x.Key)
            .ToList();

        _logger.LogInformation("Notifying agents for {RegionCount} out of {MaxRegionCount} regions", agentsToNotify.Count, allAgents.Count);

        return _agentService.NotifyAgents(agentsToNotify);
    }
}
