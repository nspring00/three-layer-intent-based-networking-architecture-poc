using Knowledge.API.Configs;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Knowledge.API.Policies;

// Source: https://www.c-sharpcorner.com/article/publishing-rabbitmq-message-in-asp-net-core/
public class RabbitModelPooledObjectPolicy : IPooledObjectPolicy<IModel>, IDisposable
{
    private readonly RabbitOptions _options;
    private readonly IConnection _connection;

    public RabbitModelPooledObjectPolicy(IOptions<RabbitOptions> optionsAccs)
    {
        _options = optionsAccs.Value;
        _connection = GetConnection();
    }

    private IConnection GetConnection()
    {
        var factory = new ConnectionFactory()
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VHost
        };

        return factory.CreateConnection();
    }

    public IModel Create()
    {
        return _connection.CreateModel();
    }

    public bool Return(IModel obj)
    {
        if (obj.IsOpen)
        {
            return true;
        }

        obj.Dispose();
        return false;
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }
}
