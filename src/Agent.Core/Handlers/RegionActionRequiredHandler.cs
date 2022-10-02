using Agent.Core.Models;
using Agent.Core.Services;
using Common.Models;
using Microsoft.Extensions.Logging;

namespace Agent.Core.Handlers
{
    public class RegionActionRequiredHandler
    {
        private readonly ILogger<RegionActionRequiredHandler> _logger;
        private readonly IReasoningService _reasoningService;
        private readonly ITopologyService _topologyService;
        private readonly INetworkLayerService _networkLayerService;

        public RegionActionRequiredHandler(
            ILogger<RegionActionRequiredHandler> logger,
            IReasoningService reasoningService,
            ITopologyService topologyService,
            INetworkLayerService networkLayerService)
        {
            _logger = logger;
            _reasoningService = reasoningService;
            _topologyService = topologyService;
            _networkLayerService = networkLayerService;
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
                    _logger.LogError("No topology found for region {Region}", region);
                    continue;
                }

                if (action.Scale == 0) continue;
                await HandleScaling(region, action.Scale, topologies[region]);
            }
        }

        private async Task HandleScaling(Region region, int scale, IDictionary<int, NlManagerTopology> topology)
        {
            const string application = "Application1";
            _logger.LogInformation("Scaling {Region} by {Scale}", region.Name, scale);

            if (scale > 0)
            {
                // Choose NL with least devices when scaling up
                var (_, nlManagerInfo) = topology.MinBy(x => x.Value.Devices.Count);

                var newNOs = Enumerable.Repeat(new NetworkObjectCreateInfo(application), scale).ToList();
                var newIds = await _networkLayerService.CreateNOsAsync(nlManagerInfo.Uri, newNOs);

                // TODO update cache?
                
                return;
            }

            // Scale < 0 -> invert
            scale = -scale;
            // Choose NL with most devices when scaling down
            var ordered = topology.OrderBy(x => x.Value.Devices.Count);

            // Remove n NOs, starting with the NL with the highest amount of devices
            foreach (var (_, nlManagerInfo) in ordered)
            {
                // Order devices by UpTime desc and take max scale
                var taken = nlManagerInfo.Devices
                    .OrderByDescending(x => x.UpTime)
                    .Take(scale)
                    .Select(x => x.Id)
                    .ToList();

                scale -= taken.Count;
                var notFoundIds = await _networkLayerService.DeleteNOsAsync(nlManagerInfo.Uri, taken);

                // TODO update cache and handle not found ids correctly
                scale += notFoundIds.Count;

                if (scale == 0) return;
            }
        }
    }
}
