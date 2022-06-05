using System.Text;
using System.Text.Json;
using Knowledge.API.Dtos;
using Knowledge.API.Services;
using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Knowledge.API.HostedServices;

public class ReasoningRequestConsumerService : IHostedService
{
    private readonly ILogger<ReasoningRequestConsumerService> _logger;
    private readonly IReasoningService _reasoningService;
    private readonly ObjectPool<IModel> _channelPool;
    private readonly JsonSerializerOptions _jsonOptions;

    private IModel? _channel;
    private string? _consumerTag;


    public ReasoningRequestConsumerService(
        ILogger<ReasoningRequestConsumerService> logger,
        IReasoningService reasoningService,
        ObjectPool<IModel> channelPool)
    {
        _logger = logger;
        _reasoningService = reasoningService;
        _channelPool = channelPool;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting ReasoningRequestConsumerService");

        // TODO get this from config
        const string queueName = "knowledge_reasoning_requests";
        _channel = _channelPool.Get();
        _channel.QueueDeclare(queueName, true, false, false, null);


        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += ConsumerOnReceived;

        _consumerTag = _channel.BasicConsume(queueName, false, consumer);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping ReasoningRequestConsumerService");
        if (_consumerTag is not null)
        {
            _channel!.BasicCancel(_consumerTag);
        }

        _channel!.Close();
        return Task.CompletedTask;
    }

    private void ConsumerOnReceived(object? sender, BasicDeliverEventArgs ea)
    {
        // TODO move to bottom afterwards
        _channel!.BasicAck(ea.DeliveryTag, false);

        var body = ea.Body.ToArray();

        var bodyString = Encoding.UTF8.GetString(body);
        //Console.WriteLine($"Received message: {bodyString}");
        var request = JsonSerializer.Deserialize<ReasoningRequest>(bodyString, _jsonOptions);
        if (request is null)
        {
            _logger.LogWarning($"Failed to deserialize message {bodyString}");
            return;
        }

        if (request.Regions.Count == 0)
        {
            _logger.LogWarning($"No regions specified for reasoning request {bodyString}");
            return;
        }

        var reasoningResults = request.Regions
            .Select(r => _reasoningService.ReasonForRegion(r))
            .ToList();

        _logger.LogInformation($"Reasoning results: {string.Join(", ", reasoningResults)}");

        // TODO logic
    }
}
