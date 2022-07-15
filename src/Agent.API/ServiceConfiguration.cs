using Agent.API.BackgroundServices;
using Agent.API.Configs;
using Agent.Core;
using Common.Web.Rabbit;

namespace Agent.API;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<RegionActionRequiredHandler>(); // TODO make this scoped / use some mediator 

        services.Configure<RabbitQueues>(configuration.GetSection("RabbitQueues")); // TODO fix config loading
        services.AddRabbitMq(configuration);
        services.AddHostedService<ActionRequestRabbitService>();
    }
}
