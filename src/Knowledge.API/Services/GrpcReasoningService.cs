using Grpc.Core;
using Knowledge.Grpc.Reasoning;

namespace Knowledge.API.Services;

public class GrpcReasoningService : Grpc.Reasoning.ReasoningService.ReasoningServiceBase
{
    private readonly IReasoningService _reasoningService;

    public GrpcReasoningService(IReasoningService reasoningService)
    {
        _reasoningService = reasoningService;
    }

    public override Task<ReasoningComposition> Reason(ReasoningRequest request, ServerCallContext context)
    {
        var response = new ReasoningComposition { RegionCompositions = { request.RegionNames.Select(HandleRegion) } };

        return Task.FromResult(response);
    }

    private RegionReasoningComposition HandleRegion(string regionName)
    {
        var result = _reasoningService.ReasonForRegion(regionName);
        return new RegionReasoningComposition
        {
            RegionName = regionName, 
            ActionRequired = result.ActionRequired, 
            Action = MapAction(result.Action)
        };
    }

    private static AgentAction? MapAction(Models.AgentAction? action)
    {
        if (action is null) return null;
        return new AgentAction { Scale = action.Scale };
    }
}
