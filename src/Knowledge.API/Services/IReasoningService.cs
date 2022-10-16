using Common.Models;
using Knowledge.API.Models;

namespace Knowledge.API.Services;

public interface IReasoningService
{
    int MaxInfosForReasoning { get; }
    
    /// <summary>
    /// Executes quick reasoning if action is required for the given regions.
    /// Uses only the current workload and intents to check against the given thresholds.
    /// Returns a list of regions that require action.
    /// </summary>
    /// <param name="regions">List of regions to check.</param>
    /// <returns>List of regions that require action.</returns>
    IDictionary<Region, bool> QuickReasoningForRegions(IList<Region> regions);
    ReasoningComposition ReasonForRegion(Region region);

    public Dictionary<KeyPerformanceIndicator, float> GenerateKpiTrends(IList<WorkloadInfo> infos,
        IList<KeyPerformanceIndicator> kpis);
}
