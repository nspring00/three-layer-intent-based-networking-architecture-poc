using Common.Models;
using Data.Console.Models;

namespace Data.Console.Services;

public interface IKnowledgeService
{
    Task UpdateKnowledgeNOs(DateTime timestamp, IDictionary<Region, NetworkUpdate> updates);
}
