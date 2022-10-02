using System.Text;
using System.Text.Json;
using Agent.API.Contracts.Messages;
using Common.Models;
using Common.Web.Rabbit.Configs;
using Knowledge.API.Configs;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Knowledge.API.Services;

public class RabbitAgentService : IAgentService
{
    private readonly ILogger<RabbitAgentService> _logger;
    private readonly ObjectPool<IModel> _channelPool;
    private readonly RabbitQueueOptions _queueOptions;

    public RabbitAgentService(
        ILogger<RabbitAgentService> logger,
        ObjectPool<IModel> channelPool,
        IOptions<RabbitQueues> rabbitQueues)
    {
        _logger = logger;
        _channelPool = channelPool;
        _queueOptions = rabbitQueues.Value.RegionActionRequiredRequests;
    }

    public Task NotifyAgents(IList<Region> regions)
    {
        using var channel = _channelPool.Get();

        // Use batch publish to publish all messages at once
        var publishBatch = channel.CreateBasicPublishBatch();

        _logger.LogInformation("Notifying {RegionsCount} regions via RabbitMQ", regions.Count);
        foreach (var region in regions)
        {
            var message = new RegionActionRequiredRequest
            {
                Region = region.Name
            };

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            var bodyRom = new ReadOnlyMemory<byte>(body);
            publishBatch.Add("", _queueOptions.QueueName, false, null, bodyRom);
        }

        publishBatch.Publish();

        _logger.LogInformation("Successfully published batch message");

        return Task.CompletedTask;
    }
}
