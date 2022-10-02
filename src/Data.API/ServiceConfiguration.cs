using Common.Services;
using Data.API.BackgroundServices;
using Data.API.Options;
using Data.API.Services;
using Data.Core.Clients;
using Data.Core.Options;
using Data.Core.Repositories;
using Data.Core.Services;

namespace Data.API;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<List<NlManagerInfoOptions>>(configuration.GetSection("NlManagers"));
        services.Configure<ExternalServiceConfig>(configuration.GetSection("ExternalServices"));

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<INlManagerService, NlManagerService>();
        services.AddSingleton<INetworkObjectRepository, NetworkObjectRepository>();
        services.AddSingleton<INetworkObjectService, NetworkObjectService>();
        services.AddSingleton<INetworkLayerService, NetworkLayerService>();
        services.AddSingleton<IKnowledgeService, KnowledgeService>();
        services.AddSingleton<IEfficiencyService, EfficiencyService>();

        services.AddSingleton<NlGrpcClient>();
        services.AddSingleton<KnowledgeGrpcClient>();

        services.AddHostedService<NoAggregationService>();

        services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = true;
        });
        services.AddGrpcReflection();
    }
}
