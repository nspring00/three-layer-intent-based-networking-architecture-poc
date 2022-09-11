using Common.Models;

namespace Knowledge.API.Services;

public interface IWorkloadAnalysisService
{
    IDictionary<Region, bool> CheckIfAgentsShouldBeNotified(IList<Region> regions);
}
