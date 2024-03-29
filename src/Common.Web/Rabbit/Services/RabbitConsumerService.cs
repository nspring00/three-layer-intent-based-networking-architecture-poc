﻿using System.Text;
using Common.Web.Rabbit.Configs;
using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Common.Web.Rabbit.Services;

public class RabbitConsumerService : IHostedService
{
    private readonly ILogger<RabbitConsumerService> _baseLogger;
    private readonly ObjectPool<IModel> _channelPool;
    private readonly RabbitQueueOptions _queueOptions;

    private IModel? _channel;
    private string? _consumerTag;

    public RabbitConsumerService(
        ILogger<RabbitConsumerService> baseLogger,
        ObjectPool<IModel> channelPool,
        RabbitQueueOptions queueOptions)
    {
        _baseLogger = baseLogger;
        _channelPool = channelPool;
        _queueOptions = queueOptions;
    }

    public virtual Task HandleMessage(string content, IDictionary<string, object> headers)
    {
        _baseLogger.LogInformation("Received message {Message}", content);
        return Task.CompletedTask;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _baseLogger.LogInformation("Starting RabbitConsumerService with Queue {QueueName}", _queueOptions.QueueName);

        _channel = _channelPool.Get();
        if (_channel is null)
        {
            _baseLogger.LogError($"Failed to create RabbitMQ channel");
            return Task.CompletedTask;
        }

        _channel.QueueDeclare(_queueOptions.QueueName, _queueOptions.Durable, _queueOptions.Exclusive,
            _queueOptions.AutoDelete, _queueOptions.Arguments);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.Span);
            await HandleMessage(content, ea.BasicProperties.Headers);
            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _consumerTag = _channel.BasicConsume(_queueOptions.QueueName, false, consumer);

        _baseLogger.LogInformation("Successfully bound to RabbitMQ queue {QueueName}", _queueOptions.QueueName);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _baseLogger.LogInformation("Stopping RabbitConsumerService");
        if (_consumerTag is not null)
        {
            _channel!.BasicCancel(_consumerTag);
        }

        _channel?.Close();
        return Task.CompletedTask;
    }
}
