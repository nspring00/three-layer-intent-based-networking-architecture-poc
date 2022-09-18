using Xunit;
using Amazon.Lambda.SQSEvents;
using Amazon.Lambda.TestUtilities;

namespace Agent.Lambda.Tests;

public class FunctionTest
{
    [Fact]
    public async Task TestToUpperFunction()
    {

        // Invoke the lambda function and confirm the string was upper cased.
        var function = new Function();
        var context = new TestLambdaContext();
        var sqsEvent = new SQSEvent
        {
            Records = new List<SQSEvent.SQSMessage>
            {
                new()
                {
                    Body = "Hello World"
                }
            }
        };
        var casing = await function.FunctionHandler(sqsEvent, context);

        Assert.Single(casing);
        Assert.Equal("hello world", casing.First().Lower);
        Assert.Equal("HELLO WORLD", casing.First().Upper);
    }
}
