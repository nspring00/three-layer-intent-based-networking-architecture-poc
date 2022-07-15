using Agent.Core.Clients;
using Agent.Core.Models;
using Common.Models;
using Microsoft.Extensions.Logging;

namespace Agent.Core.Services;

public class GrpcReasoningService : IReasoningService
{
    private readonly KnowledgeGrpcClient _client;
    private readonly ILogger<GrpcReasoningService> _logger;

    private readonly Uri _knowledgeUri = new("https://localhost:7070"); // TODO get from config

    public GrpcReasoningService(KnowledgeGrpcClient client, ILogger<GrpcReasoningService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<IDictionary<Region, AgentAction>> GetRequiredActions(IList<Region> regions)
    {
        _logger.LogInformation("Retrieving required actions for regions {Regions}",
            string.Join("", regions.Select(x => x.Name)));

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
