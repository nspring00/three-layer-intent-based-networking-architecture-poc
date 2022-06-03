using Knowledge.API.Models;
using Knowledge.API.Repository;

namespace Knowledge.API.Services;

public class ReasoningService
{
    private readonly IntentRepository _intentRepository;
    private readonly NetworkInfoRepository _networkInfoRepository;

    // TODO use interfaces here
    public ReasoningService(IntentRepository intentRepository, NetworkInfoRepository networkInfoRepository)
    {
        _intentRepository = intentRepository;
        _networkInfoRepository = networkInfoRepository;
    }

    // Check if efficiency goal is reached for region
    public void ReasonForRegion(string region)
    {
        var intents = _intentRepository.GetForRegion(region);
        var devices = _networkInfoRepository.GetForRegion(region);
        var deviceCount = devices.Count;
        
        // TODO error handling
        var utilizationSum = devices.Sum(d => d.Utilization);

        // Get min and max from intents
        var min = intents.Where(i => i.Set.TargetMode == TargetMode.Min).Min(i => i.Set.TargetValue);
        var max = intents.Where(i => i.Set.TargetMode == TargetMode.Max).Max(i => i.Set.TargetValue);

        if (utilizationSum/deviceCount > max)
        {
            // Find lowest number so that average utilization is under max
            
        }
    }
}
