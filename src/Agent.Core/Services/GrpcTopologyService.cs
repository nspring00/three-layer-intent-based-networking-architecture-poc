using Agent.Core.Clients;
using Agent.Core.Models;
using Common.Models;
using Microsoft.Extensions.Logging;

namespace Agent.Core.Services;

public class GrpcTopologyService : ITopologyService
{
    private readonly ILogger<GrpcTopologyService> _logger;
    private readonly DataGrpcClient _client;

    private readonly Uri _dataUri = new("https://localhost:7110");  // TODO get from config

    public GrpcTopologyService(ILogger<GrpcTopologyService> logger, DataGrpcClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task<IDictionary<Region, IDictionary<int, NlManagerTopology>>> GetTopologyForRegionsAsync(IList<Region> regions)
    {
        _logger.LogInformation("Retrieving topology information for regions {Regions}", string.Join(", ", regions));

        // TODO cache this??
        return await _client.GetTopologyForRegionsAsync(_dataUri, regions);
    }
}
