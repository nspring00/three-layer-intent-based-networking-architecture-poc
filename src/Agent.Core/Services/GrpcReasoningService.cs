using Agent.Core.Clients;
using Agent.Core.Models;
using Common.Models;

namespace Agent.Core.Services;

public class GrpcReasoningService : IReasoningService
{
    private readonly KnowledgeGrpcClient _client;

    private readonly Uri _knowledgeUri = new("https://localhost:7070");

    public GrpcReasoningService(KnowledgeGrpcClient client)
    {
        _client = client;
    }

    public async Task<IDictionary<Region, AgentAction>> GetRequiredActions(IList<Region> regions)
    {
        var actions = await _client.ExecuteReasoningAsync(_knowledgeUri, regions);

        // TODO validation??
        if (actions.GroupBy(a => a.Region).Any(x => x.Count() > 1))
        {
            throw new Exception("Duplicate region");
        }

        var filtered = actions
                .Where(a => a.ActionRequired)
                .ToList();
        if (filtered.Any(a => a.Action is null))
        {
            throw new Exception("Action is null when ActionRequired");
        }

        return filtered
            .ToDictionary(a => a.Region, a => a.Action!);
    }
}
