using Common.Models;
using Data.Core.Clients;
using Data.Core.Models;
using Data.Core.Options;
using Microsoft.Extensions.Options;

namespace Data.Core.Services;

public class KnowledgeService : IKnowledgeService
{
    private readonly KnowledgeGrpcClient _knowledgeGrpcClient;

    private readonly Uri _knowledgeUri;

    public KnowledgeService(KnowledgeGrpcClient knowledgeGrpcClient, IOptions<ExternalServiceConfig> externalServiceConfig)
    {
        _knowledgeGrpcClient = knowledgeGrpcClient;
        _knowledgeUri = new Uri(externalServiceConfig.Value.KnowledgeServiceUri);
    }
    
    public Task UpdateKnowledgeNOs(DateTime timestamp, IDictionary<Region, NetworkUpdate> updates)
    {
        return _knowledgeGrpcClient.UpdateKnowledge(_knowledgeUri, timestamp, updates);
    }
}
