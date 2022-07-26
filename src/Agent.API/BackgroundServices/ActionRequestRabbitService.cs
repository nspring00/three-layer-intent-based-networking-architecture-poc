using System.Text.Json;
using Agent.API.Configs;
using Agent.API.Contracts.Messages;
using Agent.Core;
using Agent.Core.Handlers;
using Common.Models;
using Common.Web.Rabbit.Services;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Agent.API.BackgroundServices;

public class ActionRequestRabbitService : RabbitConsumerService
{
    private readonly RegionActionRequiredHandler _handler;

    public ActionRequestRabbitService(
        ILogger<RabbitConsumerService> baseLogger,
        ObjectPool<IModel> channelPool,
        IOptions<RabbitQueues> rabbitQueueOptions,
        RegionActionRequiredHandler handler)
        : base(baseLogger, channelPool, rabbitQueueOptions.Value.RegionActionRequiredRequests)
    {
        _handler = handler;
    }

    public override async Task HandleMessage(string content, IDictionary<string, object> headers)
    {
        var request = JsonSerializer.Deserialize<RegionActionRequiredRequest>(content);
        if (request is null) return;

        await _handler.HandleRegions(new List<Region> { new(request.Region) });
    }
}
