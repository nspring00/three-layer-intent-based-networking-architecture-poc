using System.Text.Json;
using Agent.API.Configs;
using Agent.API.Contracts.Requests;
using Agent.Core;
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

    public override void HandleMessage(string content, IDictionary<string, object> headers)
    {
        var request = JsonSerializer.Deserialize<RegionActionRequiredRequest>(content);
        if (request is null) return;

        _handler.HandleRegions(new List<string> { request.Region });
    }
}
