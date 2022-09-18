using Agent.API.Contracts.Messages;
using Common.Models;
using Common.Sqs;
using Knowledge.API.Configs;
using Microsoft.Extensions.Options;

namespace Knowledge.API.Services;

public class SqsAgentService : IAgentService
{
    private readonly SqsPublisher _publisher;
    private readonly string _queueName;

    public SqsAgentService(SqsPublisher publisher, IOptions<SqsQueues> queues)
    {
        _publisher = publisher;
        _queueName = queues.Value.RegionActionRequiredRequestsQueue;
    }
    
    public Task NotifyAgents(IList<Region> regions)
    {
        var messages = regions.Select(x => new RegionActionRequiredRequest
        {
            Region = x.Name
        });

        return _publisher.PublishAsync(_queueName, messages);
    }
}
