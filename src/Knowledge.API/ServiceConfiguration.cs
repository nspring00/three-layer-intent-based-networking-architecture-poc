using Knowledge.API.Configs;
using Knowledge.API.HostedServices;
using Knowledge.API.Policies;
using Knowledge.API.Repository;
using Knowledge.API.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;

namespace Knowledge.API;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IIntentRepository, CachedIntentRepository>();
        services.AddSingleton<INetworkInfoRepository, CachedNetworkInfoRepository>();
        services.AddSingleton<IReasoningService, ReasoningService>(); // TODO maybe scoped?

        services.AddRabbitMq(configuration);
        services.AddHostedService<ReasoningRequestConsumerService>();
    }

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

    public static void MapGrpcServices(this WebApplication app)
    {
        app.MapGrpcService<NetworkInfoUpdateService>();
    }
}
