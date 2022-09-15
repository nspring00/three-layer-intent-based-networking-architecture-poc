//using System.Text.Json;
//using Common.Models;
//using Common.Web.Rabbit.Services;
//using Knowledge.API.Configs;
//using Knowledge.API.Contracts.Requests;
//using Knowledge.API.Services;
//using Microsoft.Extensions.ObjectPool;
//using Microsoft.Extensions.Options;
//using RabbitMQ.Client;

//namespace Knowledge.API.HostedServices;

//public class ReasoningRequestConsumerService : RabbitConsumerService
//{
//    private readonly ILogger<ReasoningRequestConsumerService> _logger;
//    private readonly IReasoningService _reasoningService;
//    private readonly JsonSerializerOptions _jsonOptions;

//    public ReasoningRequestConsumerService(ILogger<ReasoningRequestConsumerService> logger,
//        ILogger<RabbitConsumerService> baseLogger, ObjectPool<IModel> channelPool,
//        IReasoningService reasoningService,
//        IOptions<RabbitQueues> queues) : base(baseLogger, channelPool, queues.Value.ReasoningRequestQueue)
//    {
//        _logger = logger;
//        _reasoningService = reasoningService;
//        _jsonOptions = new JsonSerializerOptions
//        {
//            PropertyNameCaseInsensitive = true
//        };
//    }

//    public override Task HandleMessage(string content, IDictionary<string, object> headers)
//    {
//        try
//        {
//            var request = JsonSerializer.Deserialize<ReasoningRequest>(content, _jsonOptions);

//            if (request is null)
//            {
//                _logger.LogWarning($"Failed to deserialize message {content}");
//                return Task.CompletedTask;
//            }

//            if (request.Regions.Count == 0)
//            {
//                _logger.LogWarning($"No regions specified for reasoning request {content}");
//                return Task.CompletedTask;
//            }

//            var reasoningResults = request.Regions
//                .Select(r => _reasoningService.ReasonForRegion(new Region(r)))
//                .ToList();

//            _logger.LogInformation($"Reasoning results: {string.Join(", ", reasoningResults)}");
//        }
//        catch (JsonException e)
//        {
//            _logger.LogError(e, "");
//        }

//        return Task.CompletedTask;
//    }
//}
