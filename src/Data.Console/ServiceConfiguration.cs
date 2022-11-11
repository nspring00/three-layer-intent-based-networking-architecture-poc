using Common.Services;
using Data.Core.Clients;
using Data.Core.Repositories;
using Data.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Console;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<ServiceRunner>();
        services.AddSingleton<INetworkObjectRepository, NetworkObjectRepository>();
        services.AddSingleton<INetworkObjectService, NetworkObjectService>();
        services.AddSingleton<INetworkLayerService, NetworkLayerService>();
        services.AddSingleton<IKnowledgeService, KnowledgeService>();
        services.AddSingleton<KnowledgeGrpcClient>();
        services.AddSingleton<NlGrpcClient>();
    }
}
