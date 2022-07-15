using Agent.Core.Services;
using Common.Models;
using Microsoft.Extensions.Logging;

namespace Agent.Core
{
    public class RegionActionRequiredHandler
    {
        private readonly ILogger<RegionActionRequiredHandler> _logger;
        private readonly IReasoningService _reasoningService;

        public RegionActionRequiredHandler(ILogger<RegionActionRequiredHandler> logger, IReasoningService reasoningService)
        {
            _logger = logger;
            _reasoningService = reasoningService;
        }

        public async Task HandleRegions(IList<Region> regions)
        {
            var actions = await _reasoningService.GetRequiredActions(regions);
            if (actions.Count == 0)
            {
                _logger.LogInformation("No actions necessary");
                return;
            }


            // TODO retrieve topology for region(s) if necessary

            foreach (var (region, action) in actions)
            {
                
            }
        }
    }
}
