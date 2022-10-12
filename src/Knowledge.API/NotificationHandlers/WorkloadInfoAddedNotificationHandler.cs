using Knowledge.API.Models;
using Knowledge.API.Services;
using MediatR;

namespace Knowledge.API.NotificationHandlers;

public class WorkloadInfoAddedNotificationHandler : INotificationHandler<WorkloadInfoAddedNotification>
{
    private readonly ILogger<WorkloadInfoAddedNotificationHandler> _logger;
    private readonly IReasoningService _reasoningService;
    private readonly IAgentService _agentService;

    public WorkloadInfoAddedNotificationHandler(
        ILogger<WorkloadInfoAddedNotificationHandler> logger,
        IReasoningService reasoningService,
        IAgentService agentService)
    {
        _logger = logger;
        _reasoningService = reasoningService;
        _agentService = agentService;
    }

    public Task Handle(WorkloadInfoAddedNotification notification, CancellationToken cancellationToken)
    {
        var allAgents= _reasoningService.QuickReasoningForRegions(notification.Regions);

        var agentsToNotify = allAgents
            .Where(x => x.Value)
            .Select(x => x.Key)
            .ToList();

        if (agentsToNotify.Count == 0)
        {
            _logger.LogInformation("No agents need to be notified");
        }
        
        _logger.LogInformation("Notifying agents for {RegionCount} out of {MaxRegionCount} regions", agentsToNotify.Count, allAgents.Count);

        return _agentService.NotifyAgents(agentsToNotify);
    }
}
