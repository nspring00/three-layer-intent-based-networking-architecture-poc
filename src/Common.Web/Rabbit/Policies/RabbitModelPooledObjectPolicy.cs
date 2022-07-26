using Common.Web.Rabbit.Configs;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Common.Web.Rabbit.Policies;

// Source: https://www.c-sharpcorner.com/article/publishing-rabbitmq-message-in-asp-net-core/
public class RabbitModelPooledObjectPolicy : IPooledObjectPolicy<IModel>, IDisposable
{
    private readonly ILogger<RabbitModelPooledObjectPolicy> _logger;
    private readonly RabbitOptions _options;
    private readonly IConnection? _connection;

    public RabbitModelPooledObjectPolicy(
        ILogger<RabbitModelPooledObjectPolicy> logger,
        IOptions<RabbitOptions> optionsAccs)
    {
        _logger = logger;
        _options = optionsAccs.Value;
        _connection = GetConnection();
    }

    private IConnection? GetConnection()
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VHost
        };

        try
        {
            return factory.CreateConnection();
        }
        catch (BrokerUnreachableException)
        {
            _logger.LogError("RabbitMQ Broker is unreachable");
            //return null;
            throw;
        }
    }

    public IModel Create()
    {
        return _connection?.CreateModel()!; // TODO check
    }

    public bool Return(IModel? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (obj.IsOpen)
        {
            return true;
        }

        obj.Dispose();
        return false;
    }

    public void Dispose()
    {
        if (_connection is null) return;
        
        _connection.Close();
        _connection.Dispose();
    }
}
