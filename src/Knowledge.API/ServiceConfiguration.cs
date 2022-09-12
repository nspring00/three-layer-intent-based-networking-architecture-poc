using Common.Services;
using Common.Web.Rabbit;
using Knowledge.API.Configs;
using Knowledge.API.Repository;
using Knowledge.API.Services;
using MediatR;

namespace Knowledge.API;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IIntentRepository, CachedIntentRepository>();
        services.AddSingleton<IWorkloadRepository, CachedWorkloadRepository>();
        services.AddSingleton<IReasoningService, ReasoningService>(); // TODO maybe scoped?
        services.AddSingleton<IWorkloadAnalysisService, WorkloadAnalysisService>();


        services.Configure<RabbitQueues>(configuration.GetSection("RabbitQueues")); // TODO fix config loading
        services.AddRabbitMq(configuration);
        //services.AddHostedService<ReasoningRequestConsumerService>();

        // TODO only for local, not cloud
        services.AddSingleton<IAgentService, RabbitAgentService>();

        services.ConfigureMediatR();
    }

    public static void MapGrpcServices(this WebApplication app)
    {
        app.MapGrpcService<NetworkInfoUpdateService>();
        app.MapGrpcService<GrpcReasoningService>();
    }

    private static void ConfigureMediatR(this IServiceCollection services)
    {
        services.AddMediatR(typeof(Program));
    }
}
