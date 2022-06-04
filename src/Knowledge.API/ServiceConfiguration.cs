using Knowledge.API.Repository;
using Knowledge.API.Services;

namespace Knowledge.API;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<IIntentRepository, CachedIntentRepository>();
        services.AddSingleton<INetworkInfoRepository, CachedNetworkInfoRepository>();
        services.AddSingleton<IReasoningService, ReasoningService>(); // TODO maybe scoped?
    }
}
