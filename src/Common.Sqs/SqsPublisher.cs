using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Common.Messages;

namespace Common.Sqs;

public class SqsPublisher
{
    private readonly IAmazonSQS _sqs;

    public SqsPublisher(IAmazonSQS sqs)
    {
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
    }

    public async Task PublishAsync<TMessage>(string queueName, IEnumerable<TMessage> messages)
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
    }

}
