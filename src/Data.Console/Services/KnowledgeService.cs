using Common.Models;
using Data.Console.Clients;
using Data.Console.Models;
using Data.Console.Options;
using Microsoft.Extensions.Options;

namespace Data.Console.Services;

public class KnowledgeService : IKnowledgeService
{
    private readonly KnowledgeGrpcClient _knowledgeGrpcClient;

    private readonly Uri _knowledgeUri;

    public KnowledgeService(KnowledgeGrpcClient knowledgeGrpcClient, IOptions<ExternalServiceConfig> externalServiceConfig)
    {
        _knowledgeGrpcClient = knowledgeGrpcClient;
        _knowledgeUri = new Uri(externalServiceConfig.Value.KnowledgeServiceUri);
    }
    
    public Task UpdateKnowledgeNOs(IDictionary<Region, NetworkUpdate> updates)
    {
        return _knowledgeGrpcClient.UpdateKnowledge(_knowledgeUri, updates);
    }
}
