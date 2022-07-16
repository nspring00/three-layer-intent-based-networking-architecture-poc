using Common.Services;
using Data.API.Configs;
using Data.API.Services;
using Data.Console.Repositories;

namespace Data.API;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var nlManagerSection = configuration.GetSection("NlManagers");
        services.Configure<List<NlManagerInfoConfig>>(nlManagerSection);

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<INlManagerService, NlManagerService>();
        services.AddSingleton<INetworkObjectRepository, NetworkObjectRepository>();

        services.AddGrpc();
        services.AddGrpcReflection();
    }
}
