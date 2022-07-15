using Common.Web.Rabbit.Configs;
using Common.Web.Rabbit.Policies;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;

namespace Common.Web.Rabbit;

public static class ServiceConfiguration
{
    public static void AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        var rabbitConfig = configuration.GetSection("RabbitMQ");
        services.Configure<RabbitOptions>(rabbitConfig);
        services.AddSingleton<RabbitModelPooledObjectPolicy>();
        services.AddSingleton(serviceProvider =>
        {
            var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
            var policy = serviceProvider.GetRequiredService<RabbitModelPooledObjectPolicy>();
            return provider.Create(policy);
        });
    }
}
