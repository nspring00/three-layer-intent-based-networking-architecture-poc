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

                // var casings = new List<Casing>();
                foreach (var record in sqsEvent.Records.Where(record => record is not null))
                {
                    _logger.LogInformation("Message ID: {RecordMessageId}", record.MessageId);
                    _logger.LogInformation("Event Source: {RecordEventSource}", record.EventSource);

                    _logger.LogInformation("Record Body: {Body}", record.Body);

                    // Validation
                    // const string attributeName = nameof(RegionActionRequiredRequest.MessageTypeName);
                    // if (!record.Attributes.ContainsKey(attributeName))
                    // {
                    //     _logger.LogError("Record does not contain attribute {AttributeName}", attributeName);
                    //     continue;
                    // }
                    
                    _logger.LogInformation("Attributes: {Attributes}", string.Join(", ", record.Attributes.Select(x => $"{x.Key}: {x.Value}")));
                    record.MessageAttributes.Select(x => $"{x.Key}: {x.Value.StringValue}").ToList().ForEach(x => _logger.LogInformation("Message Attributes: {MessageAttributes}", x));
                    
                    var handleRequest = JsonSerializer.Deserialize<RegionActionRequiredRequest>(record.Body);
                    if (handleRequest is null)
                    {
                        _logger.LogError("Unable to deserialize message body to {MessageTypeName}", nameof(RegionActionRequiredRequest));
                        continue;
                    }

                    // if (record.Attributes[attributeName] != handleRequest.MessageTypeName)
                    // {
                    //     _logger.LogError("Record attribute {AttributeName} does not match message type name {MessageTypeName}", attributeName, handleRequest.MessageTypeName);
                    //     continue;
                    // }

                    // Handle
                    // var host = record.Body;
                    // var uri = new Uri($"http://{host}");
                    // _logger.LogInformation("Uri: {Uri}", uri);

                    await _handler.HandleRegions(new List<Region> { new(handleRequest.Region) });
                    
                    // var client = new NetworkLayerGrpcClient();
                    // try
                    // {
                    //     var response = await client.ScaleUp(uri, new List<NetworkObjectCreateInfo> { new("app17") });
                    //     _logger.LogInformation("Response: {Response}", string.Join(", ", response));
                    // }
                    // catch (Exception e)
                    // {
                    //     _logger.LogError(e, "Error while calling ScaleUp");
                    // }

                    // var casing = new Casing(record.Body.ToLower(), record.Body.ToUpper());
                    // casings.Add(casing);
                }

                _logger.LogInformation("Processing complete");

                // return casings;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Could not do stuff");
                throw;
            }
        }
    }
}
