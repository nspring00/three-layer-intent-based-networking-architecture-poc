using Knowledge.API.Models;
using Knowledge.API.Repository;

namespace Knowledge.API.Services;

public class ReasoningService : IReasoningService
{
    private readonly ILogger<ReasoningService> _logger;
    private readonly IIntentRepository _intentRepository;
    private readonly INetworkInfoRepository _networkInfoRepository;

    // TODO use interfaces here
    public ReasoningService(
        ILogger<ReasoningService> logger,
        IIntentRepository intentRepository, 
        INetworkInfoRepository networkInfoRepository)
    {
        _logger = logger;
        _intentRepository = intentRepository;
        _networkInfoRepository = networkInfoRepository;
    }

    // Check if efficiency goal is reached for region
    public ReasoningComposition ReasonForRegion(string region)
    {
        _logger.LogInformation("Reasoning for region {Region}", region);
        
        var intents = _intentRepository.GetForRegion(region);
        var devices = _networkInfoRepository.GetForRegion(region);
        var deviceCount = devices.Count;
        
        // TODO error handling
        var utilizationSum = devices.Sum(d => d.Utilization);
        var avg = utilizationSum / deviceCount;

        // Get min and max from intents
        var min = intents.FirstOrDefault(i => i.Set.TargetMode == TargetMode.Min);
        var max = intents.FirstOrDefault(i => i.Set.TargetMode == TargetMode.Max);

        _logger.LogInformation("Min: {Min}, Max: {Max}, Avg: {Avg}, Count: {Count}", min?.Set.TargetValue, max?.Set.TargetValue, avg, deviceCount);

        
        //if (min is not null && max is not null && (avg < min.Set.TargetValue || avg > max.Set.TargetValue))
        //{
        //    // If both min and max are set, target their mean
        //    var target = (min.Set.TargetValue + max.Set.TargetValue) / 2f;
        //    var delta = (int)Math.Round(CalculateDelta(deviceCount, avg, target));
        //    _logger.LogInformation("Min/Max Delta: {Delta}", delta);
        //    return new ReasoningComposition(true, new AgentAction(delta));
        //}
        
        if (max is not null && avg > max.Set.TargetValue)
        {
            // If only max, then scale up minimum required amount (ceil)
            var delta = (int) Math.Ceiling(CalculateDelta(deviceCount, avg, max.Set.TargetValue));
            _logger.LogInformation("Max Delta: {Delta}", delta);
            return new ReasoningComposition(true, new AgentAction(delta));
        }

        if (min is not null && avg < min.Set.TargetValue)
        {
            // If only min, then scale down minimum required amount (floor)
            var delta = (int)Math.Floor(CalculateDelta(deviceCount, avg, min.Set.TargetValue));
            _logger.LogInformation("Min Delta: {Delta}", delta);
            return new ReasoningComposition(true, new AgentAction(delta));
        }

        return new ReasoningComposition(false);
    }

    // Formula see notion
    private static float CalculateDelta(int count, float currentAvg, float targetAvg)
    {
        return count * (currentAvg / targetAvg - 1);
    }
}
