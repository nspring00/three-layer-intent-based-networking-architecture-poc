using Common.Web.Rabbit;
using Knowledge.API.Configs;
using Knowledge.API.HostedServices;
using Knowledge.API.Repository;
using Knowledge.API.Services;

namespace Knowledge.API;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IIntentRepository, CachedIntentRepository>();
        services.AddSingleton<INetworkInfoRepository, CachedNetworkInfoRepository>();
        services.AddSingleton<IReasoningService, ReasoningService>(); // TODO maybe scoped?
        services.Configure<RabbitQueues>(configuration.GetSection("RabbitQueues")); // TODO fix config loading
        services.AddRabbitMq(configuration); 
        services.AddHostedService<ReasoningRequestConsumerService>();
    }

    public static void MapGrpcServices(this WebApplication app)
    {
        app.MapGrpcService<NetworkInfoUpdateService>();
        app.MapGrpcService<GrpcReasoningService>();
    }
}
