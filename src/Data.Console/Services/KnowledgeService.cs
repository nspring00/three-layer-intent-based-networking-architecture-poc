using Common.Models;
using Data.Console.Clients;
using Data.Console.Models;

namespace Data.Console.Services;

public class KnowledgeService : IKnowledgeService
{
    private readonly KnowledgeGrpcClient _knowledgeGrpcClient;

    private readonly Uri _knowledgeUri = new Uri("https://localhost:7070");

    public KnowledgeService(KnowledgeGrpcClient knowledgeGrpcClient)
    {
        _knowledgeGrpcClient = knowledgeGrpcClient;
    }
    
    public Task UpdateKnowledgeNOs(IDictionary<Region, NetworkUpdate> updates)
    {
        return _knowledgeGrpcClient.UpdateKnowledge(_knowledgeUri, updates);
    }
}
