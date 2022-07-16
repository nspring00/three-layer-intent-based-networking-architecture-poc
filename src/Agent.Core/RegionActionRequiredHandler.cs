using Agent.Core.Clients;
using Agent.Core.Models;
using Agent.Core.Services;
using Common.Models;
using Data.Grpc.Topology;
using Microsoft.Extensions.Logging;

namespace Agent.Core
{
    public class RegionActionRequiredHandler
    {
        private readonly ILogger<RegionActionRequiredHandler> _logger;
        private readonly IReasoningService _reasoningService;
        private readonly ITopologyService _topologyService;

        public RegionActionRequiredHandler(
            ILogger<RegionActionRequiredHandler> logger,
            IReasoningService reasoningService,
            ITopologyService topologyService)
        {
            _logger = logger;
            _reasoningService = reasoningService;
            _topologyService = topologyService;
        }

        public async Task HandleRegions(IList<Region> regions)
        {
            var actions = await _reasoningService.GetRequiredActions(regions);
            if (actions.Count == 0)
            {
                _logger.LogInformation("No actions necessary");
                return;
            }


            var topologies = await _topologyService.GetTopologyForRegionsAsync(regions);

            foreach (var (region, action) in actions)
            {
                if (!topologies.ContainsKey(region))
                {
                    _logger.LogError($"No topology found for region {region}");
                    continue;
                }

                if (action.Scale == 0) continue;
                await HandleScaling(region, action.Scale, topologies[region]);
            }
        }

        private async Task HandleScaling(Region region, int scale, IDictionary<int, NlManagerTopology> topology)
        {
            _logger.LogInformation("Scaling {Region} by {Scale}", region.Name, scale);

            if (scale > 0)
            {
                // Choose NL with least devices when scaling up
                var (nlId, nlManagerInfo) = topology.MinBy(x => x.Value.Devices.Count);
                var uri = nlManagerInfo.Uri;

                
            }
        }
    }
}
