namespace Knowledge.API.Configs;

public class RabbitQueueOptions
{
    public string ReasoningRequestQueueName { get; set; } = default!;
    public string ReasoningResponseQueueName { get; set; } = default!;
}
