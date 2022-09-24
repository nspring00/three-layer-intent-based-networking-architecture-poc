using Agent.Core.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Agent.Lambda;

// https://medium.com/@piotrkarpaa/lambda-logging-in-asp-net-core-d6fe148c2760
public class Startup
{
    public IServiceProvider Setup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        var services = new ServiceCollection();
        services.AddScoped(_ => configuration);
        services.AddSingleton<ProgramEntryPoint>();
        services.AddSingleton<RegionActionRequiredHandler>();

        services.AddLogging(SetupLogger);

        IServiceProvider provider = services.BuildServiceProvider();

        return provider;
    }

    public static void SetupLogger(ILoggingBuilder logging)
    {
        if (logging == null)
        {
            throw new ArgumentNullException(nameof(logging));
        }

        // Create and populate LambdaLoggerOptions object
        var loggerOptions = new LambdaLoggerOptions
        {
            IncludeCategory = true,
            IncludeLogLevel = true,
            IncludeNewline = true,
            IncludeEventId = true,
            IncludeException = true
        };

        // Configure Lambda logging
        logging.AddLambdaLogger(loggerOptions);

        logging.SetMinimumLevel(LogLevel.Debug);
    }

}
