using Common.Web.Rabbit.Configs;

namespace Knowledge.API.Configs;

public class RabbitQueues
{
    public RabbitQueueOptions ReasoningRequestQueue { get; set; } = new()
    {
        QueueName = "knowledge_reasoning_requests"
    };

    public RabbitQueueOptions ReasoningResponseQueue { get; set; } = new()
    {
        QueueName = "agent_event_queue"
    };
}
