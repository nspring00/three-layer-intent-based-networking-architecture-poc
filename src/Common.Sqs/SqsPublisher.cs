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
        var queueUrl = await _sqs.GetQueueUrlAsync(queueName);
        var request = new SendMessageRequest
        {
            QueueUrl = queueUrl.QueueUrl,
            MessageBody = JsonSerializer.Serialize(message),
            MessageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                {
                    nameof(IMessage.MessageTypeName), new MessageAttributeValue
                    {
                        StringValue = message.MessageTypeName,
                        DataType = "String"
                    }
                }
            }
        };
        await _sqs.SendMessageAsync(request);
        
        _logger.LogInformation("Published message to queue {QueueName}", queueName);
    }

    public async Task PublishAsync<TMessage>(string queueName, ICollection<TMessage> messages)
        where TMessage : IMessage
    {
        var queueUrl = await _sqs.GetQueueUrlAsync(queueName);
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
                        nameof(IMessage.MessageTypeName), new MessageAttributeValue
                        {
                            StringValue = message.MessageTypeName,
                            DataType = "String"
                        }
                    }
                }
            }).ToList()
        };
        
        await _sqs.SendMessageBatchAsync(request);
        
        _logger.LogInformation("Published {MessageCount} messages to queue {QueueName}", messages.Count, queueName);
    }

}
