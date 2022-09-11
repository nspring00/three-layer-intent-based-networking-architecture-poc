using Knowledge.API.Models;
using Knowledge.API.Services;
using MediatR;

namespace Knowledge.API.NotificationHandlers;

public class WorkloadInfoAddedNotificationHandler : INotificationHandler<WorkloadInfoAddedNotification>
{
    private readonly IWorkloadAnalysisService _workloadAnalysisService;

    public WorkloadInfoAddedNotificationHandler(IWorkloadAnalysisService workloadAnalysisService)
    {
        _workloadAnalysisService = workloadAnalysisService;
    }

    public Task Handle(WorkloadInfoAddedNotification notification, CancellationToken cancellationToken)
    {
        var agentsToNotify = _workloadAnalysisService.CheckIfAgentsShouldBeNotified(notification.Regions);

        


        return Task.CompletedTask;
    }
}
