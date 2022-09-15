using Common.Models;

namespace Knowledge.API.Services;

public interface IAgentService
{
    Task NotifyAgents(IList<Region> regions);
}
