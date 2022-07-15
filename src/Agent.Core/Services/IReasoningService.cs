using Agent.Core.Models;
using Common.Models;

namespace Agent.Core.Services;

public interface IReasoningService
{
    Task<IDictionary<Region, AgentAction>> GetRequiredActions(IList<Region> regions);
}
