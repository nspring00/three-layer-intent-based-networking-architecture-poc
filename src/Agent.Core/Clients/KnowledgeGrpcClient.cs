using Common.Grpc;
using Common.Models;
using Knowledge.Grpc.Reasoning;
using Microsoft.Extensions.Logging;
using AgentAction = Agent.Core.Models.AgentAction;
using ReasoningComposition = Agent.Core.Models.ReasoningComposition;

namespace Agent.Core.Clients;
public class KnowledgeGrpcClient : CachedGrpcClient
{
    private readonly ILogger<KnowledgeGrpcClient> _logger;

    public KnowledgeGrpcClient(ILogger<KnowledgeGrpcClient> logger)
    {
        _logger = logger;
    }

    public async Task<IList<ReasoningComposition>> ExecuteReasoningAsync(Uri uri, IList<Region> regions)
    {
        var channel = GetChannel(uri);
        var client = new ReasoningService.ReasoningServiceClient(channel);

        var request = new ReasoningRequest
        {
            RegionNames = { regions.Select(x => x.Name) }
        };

        var response = await client.ReasonAsync(request);
        if (response is null)
        {
            _logger.LogError("gRpc response is null");
            return new List<ReasoningComposition>(0);
        }

        return response.RegionCompositions.Select(MapReasoningComposition).ToList();
    }

    private static ReasoningComposition MapReasoningComposition(RegionReasoningComposition? comp)
    {
        if (comp is null) throw new ArgumentNullException(nameof(comp));
        return new ReasoningComposition(new Region(comp.RegionName), comp.ActionRequired, MapAction(comp.Action));
    }

    private static AgentAction? MapAction(Knowledge.Grpc.Reasoning.AgentAction? action)
    {
        if (action is null) return null;
        return new AgentAction(action.Scale);
    }
}
