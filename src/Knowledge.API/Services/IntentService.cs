using Common.Models;
using Knowledge.API.Models;
using Knowledge.API.Repository;

namespace Knowledge.API.Services;

public class IntentService : IIntentService
{
    private readonly IIntentRepository _intentRepository;

    public IntentService(IIntentRepository intentRepository)
    {
        _intentRepository = intentRepository;
    }
    
    public IDictionary<KeyPerformanceIndicator, MinMaxTarget> GetKpiTargetsForRegion(Region region)
    {
        return _intentRepository.GetForRegion(region)
            .GroupBy(x => x.Kpi)
            .ToDictionary(x => x.Key, x =>
            {
                var min = x.FirstOrDefault(k => k.TargetMode == TargetMode.Min)?.TargetValue;
                var max = x.FirstOrDefault(k => k.TargetMode == TargetMode.Max)?.TargetValue;
                return new MinMaxTarget(min, max);
            });
    }
}
