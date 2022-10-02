using Amazon;
using Amazon.SQS;
using Common.Services;
using Common.Sqs;
using Common.Web.AspNetCore;
using Common.Web.Rabbit;
using Knowledge.API.Configs;
using Knowledge.API.Repository;
using Knowledge.API.Services;
using MediatR;

namespace Knowledge.API;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IIntentRepository, CachedIntentRepository>();
        services.AddSingleton<IWorkloadRepository, CachedWorkloadRepository>();
        services.AddSingleton<IReasoningService, ReasoningService>(); // TODO maybe scoped?
        services.AddSingleton<IIntentService, IntentService>();

        if (environment.IsDevelopment() || environment.IsDocker())
        {
            // Local dev and Docker
            services.Configure<RabbitQueues>(configuration.GetSection("RabbitQueues")); // TODO fix config loading
            services.AddRabbitMq(configuration);
            //services.AddHostedService<ReasoningRequestConsumerService>();
            services.AddSingleton<IAgentService, RabbitAgentService>();
        }
        else
        {
            // AWS cloud deployment
            services.AddSingleton<IAmazonSQS>(_ => new AmazonSQSClient(RegionEndpoint.USEast1));
            services.AddSingleton<SqsPublisher>();
            services.Configure<SqsQueues>(configuration.GetSection("SqsQueues"));
            services.AddSingleton<IAgentService, SqsAgentService>();
        }
        
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
