using Agent.API.BackgroundServices;
using Agent.API.Configs;
using Agent.Core.Clients;
using Agent.Core.Handlers;
using Agent.Core.Options;
using Agent.Core.Services;
using Common.Web.Rabbit;

namespace Agent.API;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ExternalServiceConfig>(configuration.GetSection("ExternalServices"));

        services.AddSingleton<RegionActionRequiredHandler>(); // TODO make this scoped / use some mediator 
        services.AddSingleton<KnowledgeGrpcClient>();
        services.AddSingleton<DataGrpcClient>();
        services.AddSingleton<NetworkLayerGrpcClient>();
        services.AddSingleton<IReasoningService, GrpcReasoningService>();
        services.AddSingleton<ITopologyService, GrpcTopologyService>();
        services.AddSingleton<INetworkLayerService, GrpcNetworkLayerService>();

        services.Configure<RabbitQueues>(configuration.GetSection("RabbitQueues")); // TODO fix config loading
        services.AddRabbitMq(configuration);
        services.AddHostedService<ActionRequestRabbitService>();
    }
}
