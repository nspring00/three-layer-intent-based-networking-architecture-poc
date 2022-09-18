using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Agent.Lambda;

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

        // TODO register services here
        
        // TODO maybe set isDevelopment to false at some point
        services.AddLogging(logging => SetupLogger(true, logging, configuration));

        IServiceProvider provider = services.BuildServiceProvider();

        return provider;
    }

    public static void SetupLogger(bool isDevelopment, ILoggingBuilder logging, IConfiguration configuration)
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

        if (isDevelopment)
        {
            logging.AddConsole();
        }
    }

}
