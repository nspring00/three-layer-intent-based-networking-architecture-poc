using Agent.Core.Clients;
using Agent.Core.Models;
using Agent.Core.Options;
using Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Agent.Core.Services;

public class GrpcReasoningService : IReasoningService
{
    private readonly ILogger<GrpcReasoningService> _logger;
    private readonly KnowledgeGrpcClient _client;
    private readonly Uri _reasoningUri;

    public GrpcReasoningService(ILogger<GrpcReasoningService> logger, KnowledgeGrpcClient client,
        IOptions<ExternalServiceConfig> serviceOptions)
    {
        _client = client;
        _logger = logger;
        _reasoningUri = new Uri(serviceOptions.Value.ReasoningServiceUri);
        logger.LogInformation("GrpcReasoningService configured with Reasoning Uri {ReasoningUri}", _reasoningUri);
    }

    public async Task<IDictionary<Region, AgentAction>> GetRequiredActions(IList<Region> regions)
    {
        _logger.LogInformation("Retrieving required actions for regions {Regions}",
            string.Join("", regions.Select(x => x.Name)));

        var actions = await _client.ExecuteReasoningAsync(_reasoningUri, regions);

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
