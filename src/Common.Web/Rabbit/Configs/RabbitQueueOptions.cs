namespace Common.Web.Rabbit.Configs;

public class RabbitQueueOptions
{
    public string QueueName { get; set; } = default!;
    public bool Durable { get; set; } = true;
    public bool Exclusive { get; set; }
    public bool AutoDelete { get; set; }
    public IDictionary<string, object>? Arguments { get; set; }
}
