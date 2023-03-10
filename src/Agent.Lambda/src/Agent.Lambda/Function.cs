using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.DependencyInjection;

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
    public Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
    {
        return _programEntryPoint.HandleSqsEvent(sqsEvent, context);
    }
}
