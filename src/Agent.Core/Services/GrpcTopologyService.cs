using Agent.Core.Clients;
using Agent.Core.Models;
using Agent.Core.Options;
using Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Agent.Core.Services;

public class GrpcTopologyService : ITopologyService
{
    private readonly ILogger<GrpcTopologyService> _logger;
    private readonly DataGrpcClient _client;
    private readonly Uri _topologyUri;

    public GrpcTopologyService(ILogger<GrpcTopologyService> logger, DataGrpcClient client, IOptions<ExternalServiceConfig> serviceOptions)
    {
        _logger = logger;
        _client = client;
        _topologyUri = new Uri(serviceOptions.Value.TopologyServiceUri);
        logger.LogInformation("GrpcTopologyService configured with Topology Uri {TopologyUri}", _topologyUri);
    }

    public async Task<IDictionary<Region, IDictionary<int, NlManagerTopology>>> GetTopologyForRegionsAsync(IList<Region> regions)
    {
        _logger.LogInformation("Retrieving topology information for regions {Regions}", string.Join(", ", regions));

        // TODO cache this??
        return await _client.GetTopologyForRegionsAsync(_topologyUri, regions);
    }
}
