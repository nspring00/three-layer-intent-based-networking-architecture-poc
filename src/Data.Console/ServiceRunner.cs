using Data.Console.Clients;
using Microsoft.Extensions.Logging;

namespace Data.Console;

public class ServiceRunner
{
    private readonly ILogger<ServiceRunner> _logger;
    private readonly NlGrpcClient _nlGrpcClient;

    public ServiceRunner(ILogger<ServiceRunner> logger, NlGrpcClient nlGrpcClient)
    {
        _logger = logger;
        _nlGrpcClient = nlGrpcClient;
    }
    
    public async Task Run()
    {
        _logger.LogInformation("Hello, World!");
        var response = await _nlGrpcClient.FetchUpdates(new Uri("https://localhost:7071"));
        
        
    }
}
