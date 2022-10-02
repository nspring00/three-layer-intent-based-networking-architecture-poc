using System.Text.Json;
using Agent.API.Contracts.Messages;
using Agent.Core.Handlers;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Common.Models;
using Microsoft.Extensions.Logging;

namespace Agent.Lambda;

public class ProgramEntryPoint
{
    private readonly ILogger<ProgramEntryPoint> _logger;
    private readonly RegionActionRequiredHandler _handler;

    public ProgramEntryPoint(ILogger<ProgramEntryPoint> logger, RegionActionRequiredHandler handler)
    {
        this._logger = logger;
        _handler = handler;
    }
    
    public async Task HandleSqsEvent(SQSEvent sqsEvent, ILambdaContext context)
    {
        using (_logger.BeginScope(context.AwsRequestId))
        {
            try
            {
                _logger.LogInformation("Beginning to process {RecordsCount} records...", sqsEvent.Records.Count);

                foreach (var record in sqsEvent.Records.Where(record => record is not null))
                {
                    _logger.LogInformation("Message ID: {RecordMessageId}", record.MessageId);

                    // Validation
                    const string attributeName = nameof(RegionActionRequiredRequest.MessageTypeName);
                    if (!record.MessageAttributes.ContainsKey(attributeName))
                    {
                        _logger.LogError("Record does not contain attribute {AttributeName}", attributeName);
                        continue;
                    }
                    
                    var handleRequest = JsonSerializer.Deserialize<RegionActionRequiredRequest>(record.Body);
                    if (handleRequest is null)
                    {
                        _logger.LogError("Unable to deserialize message body to {MessageTypeName}", nameof(RegionActionRequiredRequest));
                        continue;
                    }

                    if (record.MessageAttributes[attributeName].StringValue != handleRequest.MessageTypeName)
                    {
                        _logger.LogError("Record attribute {AttributeName} does not match message type name {MessageTypeName}", attributeName, handleRequest.MessageTypeName);
                        continue;
                    }

                    // Handle
                    await _handler.HandleRegions(new List<Region> { new(handleRequest.Region) });
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Could not do stuff");
                throw;
            }
        }
    }
}
