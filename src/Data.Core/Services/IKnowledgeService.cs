using Common.Models;
using Data.Core.Models;

namespace Data.Core.Services;

public interface IKnowledgeService
{
    Task UpdateKnowledgeNOs(DateTime timestamp, IDictionary<Region, NetworkUpdate> updates);
}
