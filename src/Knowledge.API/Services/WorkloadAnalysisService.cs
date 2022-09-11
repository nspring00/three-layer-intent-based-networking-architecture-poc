using Common.Models;
using Knowledge.API.Repository;

namespace Knowledge.API.Services;

public class WorkloadAnalysisService : IWorkloadAnalysisService
{
    private readonly IWorkloadRepository _workloadRepository;

    public WorkloadAnalysisService(IWorkloadRepository workloadRepository)
    {
        _workloadRepository = workloadRepository;
    }

    public IDictionary<Region, bool> CheckIfAgentsShouldBeNotified(IList<Region> regions)
    {
        return regions.ToDictionary(x => x, _ => false);
    }
}
