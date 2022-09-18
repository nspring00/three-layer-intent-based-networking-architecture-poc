using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Agent.Lambda;

public class Function
{
    private readonly ProgramEntryPoint _programEntryPoint;

    public Function()
    {
        var startup = new Startup();
        var provider = startup.Setup();
        
        _programEntryPoint = provider.GetRequiredService<ProgramEntryPoint>();

    }
    
    /// <summary>
    /// A simple function that takes a string and returns both the upper and lower case version of the string.
    /// </summary>
    /// <param name="sqsEvent"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public Task<List<Casing>> FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
    {
        return _programEntryPoint.HandleSqsEvent(sqsEvent, context);

        // const string reasoningServiceUri = "todo";
        // const string topologyServiceUri = "todo";
        //
        // var handler = new RegionActionRequiredHandler(
        //     new LoggerWrapper<RegionActionRequiredHandler>(logger),
        //     new GrpcReasoningService(
        //         new LoggerWrapper<GrpcReasoningService>(logger),
        //         new KnowledgeGrpcClient(new LoggerWrapper<KnowledgeGrpcClient>(logger)),
        //         new OptionsWrapper<ExternalServiceConfig>(new ExternalServiceConfig()
        //         {
        //             ReasoningServiceUri = reasoningServiceUri, TopologyServiceUri = topologyServiceUri
        //         })),
        //     new GrpcTopologyService(),
        //     new GrpcNetworkLayerService()
        // );
    }
}

public class ProgramEntryPoint
{
    private readonly ILogger<ProgramEntryPoint> _logger;

    public ProgramEntryPoint(ILogger<ProgramEntryPoint> logger)
    {
        this._logger = logger;
    }
    
    public Task<List<Casing>> HandleSqsEvent(SQSEvent sqsEvent, ILambdaContext context)
    {
        using (_logger.BeginScope(context.AwsRequestId))
        {
            try
            {
                _logger.LogInformation("Beginning to process {RecordsCount} records...", sqsEvent.Records.Count);

                var casings = new List<Casing>();
                foreach (var record in sqsEvent.Records)
                {
                    if (record is null) continue;
            
                    _logger.LogInformation("Message ID: {RecordMessageId}", record.MessageId);
                    _logger.LogInformation("Event Source: {RecordEventSource}", record.EventSource);

                    _logger.LogInformation("Record Body:");
                    _logger.LogInformation(record.Body);
            
                    var casing = new Casing(record.Body.ToLower(), record.Body.ToUpper());
                    casings.Add(casing);
                }

                _logger.LogInformation("Processing complete");
        
                return Task.FromResult(casings);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Could not do stuff");
                throw;
            }
        }
    }
}

public record Casing(string Lower, string Upper);
