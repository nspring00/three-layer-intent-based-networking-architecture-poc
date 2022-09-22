using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Common.Messages;
using Microsoft.Extensions.Logging;

namespace Common.Sqs;

public class SqsPublisher
{
    private readonly ILogger<SqsPublisher> _logger;
    private readonly IAmazonSQS _sqs;

    public SqsPublisher(ILogger<SqsPublisher> logger, IAmazonSQS sqs)
    {
        _logger = logger;
        _sqs = sqs;
    }

    public async Task PublishAsync<TMessage>(string queueName, TMessage message)
        where TMessage : IMessage
    {
        _logger.LogInformation("Publishing message to queue {QueueName}", queueName);

        var queueUrl = await _sqs.GetQueueUrlAsync(queueName);

        _logger.LogInformation("Queue URL is {QueueUrl}", queueUrl.QueueUrl);

        var request = new SendMessageRequest
        {
            QueueUrl = queueUrl.QueueUrl,
            MessageBody = JsonSerializer.Serialize(message),
            MessageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                {
                    nameof(IMessage.MessageTypeName),
                    new MessageAttributeValue { StringValue = message.MessageTypeName, DataType = "String" }
                }
            }
        };
        var response = await _sqs.SendMessageAsync(request);

        _logger.LogInformation("Published message {ResponseId} to queue {QueueName}: {ResponseCode}",
            response.MessageId, queueName, response.HttpStatusCode);
    }

    public async Task PublishAsync<TMessage>(string queueName, ICollection<TMessage> messages)
        where TMessage : IMessage
    {
        _logger.LogInformation("Publishing {MessageCount} messages to queue {QueueName}", messages.Count, queueName);

        var queueUrl = await _sqs.GetQueueUrlAsync(queueName);

        _logger.LogInformation("Queue URL is {QueueUrl}", queueUrl.QueueUrl);

        var request = new SendMessageBatchRequest
        {
            QueueUrl = queueUrl.QueueUrl,
            Entries = messages.Select((message, index) => new SendMessageBatchRequestEntry
            {
                Id = index.ToString(),
                MessageBody = JsonSerializer.Serialize(message),
                MessageAttributes = new Dictionary<string, MessageAttributeValue>
                {
                    {
                        nameof(IMessage.MessageTypeName),
                        new MessageAttributeValue { StringValue = message.MessageTypeName, DataType = "String" }
                    }
                }
            }).ToList()
        };

        var response = await _sqs.SendMessageBatchAsync(request);

        response.Failed.ForEach(failure =>
        {
            _logger.LogError(
                "Failed to publish message {MessageId} to queue {QueueName}: {FailureCode} {FailureSenderFault} {FailureMessage}",
                failure.Id, queueName, failure.Code, failure.SenderFault, failure.Message);
        });

        _logger.LogInformation("Published {MessageCount} messages to queue {QueueName}", messages.Count, queueName);
    }
}
