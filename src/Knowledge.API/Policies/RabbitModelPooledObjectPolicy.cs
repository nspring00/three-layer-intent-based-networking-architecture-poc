using Knowledge.API.Configs;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Knowledge.API.Policies;

// Source: https://www.c-sharpcorner.com/article/publishing-rabbitmq-message-in-asp-net-core/
public class RabbitModelPooledObjectPolicy : IPooledObjectPolicy<IModel>, IDisposable
{
    private readonly ILogger<RabbitModelPooledObjectPolicy> _logger;
    private readonly RabbitOptions _options;
    private readonly IConnection? _connection;
    private bool _connected = false;

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
        var factory = new ConnectionFactory()
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VHost
        };

        try
        {
            var conn = factory.CreateConnection();
            _connected = true;
            return conn;
        } catch (BrokerUnreachableException e)
        {
            _logger.LogError(e, "RabbitMQ broker is unreachable");
            return null;
        }
    }

    public IModel? Create()
    {
        if (_connection is null)
        {
            return null;
        }
        return _connection.CreateModel();
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
        if (_connection is null)
        {
            return;
        }
        
        _connection.Close();
        _connection.Dispose();
    }
}
