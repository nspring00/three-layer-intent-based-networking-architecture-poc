using Common.Models;
using Knowledge.API.Models;

namespace Knowledge.API.Services;

public interface IReasoningService
{
    /// <summary>
    /// Executes quick reasoning if action is required for the given regions.
    /// Uses only the current workload and intents to check against the given thresholds.
    /// Returns a list of regions that require action.
    /// </summary>
    /// <param name="regions">List of regions to check.</param>
    /// <returns>List of regions that require action.</returns>
    IDictionary<Region, bool> QuickReasoningForRegions(IList<Region> regions);
    ReasoningComposition ReasonForRegion(Region region);
}
