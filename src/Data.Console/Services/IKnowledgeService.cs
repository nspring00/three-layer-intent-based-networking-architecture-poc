using Data.Console.Models;

namespace Data.Console.Services;

public interface IKnowledgeService
{
    Task UpdateKnowledgeNOs(IDictionary<Region, NetworkUpdate> updates);
}
