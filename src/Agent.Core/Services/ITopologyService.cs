using Agent.Core.Models;
using Common.Models;

namespace Agent.Core.Services;
public interface ITopologyService
{
    Task<IDictionary<Region, IDictionary<int, NlManagerTopology>>> GetTopologyForRegionsAsync(IList<Region> regions);
}
