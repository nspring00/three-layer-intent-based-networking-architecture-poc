using Common.Models;
using Data.Core.Clients;
using Data.Core.Models;
using Data.Core.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Data.Core.Services;

public class KnowledgeService : IKnowledgeService
{
    private readonly ILogger<KnowledgeService> _logger;
    private readonly KnowledgeGrpcClient _knowledgeGrpcClient;

    private readonly Uri _knowledgeUri;

    public KnowledgeService(
        ILogger<KnowledgeService> logger,
        KnowledgeGrpcClient knowledgeGrpcClient, 
        IOptions<ExternalServiceConfig> externalServiceConfig)
    {
        _logger = logger;
        _knowledgeGrpcClient = knowledgeGrpcClient;
        _knowledgeUri = new Uri(externalServiceConfig.Value.KnowledgeServiceUri);
    }
    
    public async Task UpdateKnowledgeNOs(DateTime timestamp, IDictionary<Region, NetworkUpdate> updates)
    {
        try
        {
            await _knowledgeGrpcClient.UpdateKnowledge(_knowledgeUri, timestamp, updates);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to update knowledge");
        }
    }
}
