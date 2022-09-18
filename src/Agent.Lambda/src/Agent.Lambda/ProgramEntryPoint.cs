using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.Logging;

namespace Agent.Lambda;

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
