namespace Common.Web.Rabbit.Configs;

public class RabbitOptions
{
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string HostName { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 5672;
    public string VHost { get; set; } = "/";
}
