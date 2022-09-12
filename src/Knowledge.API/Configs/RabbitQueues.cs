using Common.Web.Rabbit.Configs;

namespace Knowledge.API.Configs;

public class RabbitQueues
{
    public RabbitQueueOptions RegionActionRequiredRequests { get; set; } = new()
    {
        QueueName = "region_action_required_requests"
    };
}
