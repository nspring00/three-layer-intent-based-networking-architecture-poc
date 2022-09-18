using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Agent.Lambda;

public class Function
{
    /// <summary>
    /// A simple function that takes a string and returns both the upper and lower case version of the string.
    /// </summary>
    /// <param name="sqsEvent"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public List<Casing> FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
    {
        var logger = context.Logger;
        
        logger.LogInformation($"Beginning to process {sqsEvent.Records.Count} records...");

        var casings = new List<Casing>();
        foreach (var record in sqsEvent.Records)
        {
            if (record is null) continue;
            
            logger.LogInformation($"Message ID: {record.MessageId}");
            logger.LogInformation($"Event Source: {record.EventSource}");

            logger.LogInformation("Record Body:");
            logger.LogInformation(record.Body);
            
            var casing = new Casing(record.Body.ToLower(), record.Body.ToUpper());
            casings.Add(casing);
        }

        logger.LogInformation("Processing complete");
        
        return casings;
    }
}

public record Casing(string Lower, string Upper);
