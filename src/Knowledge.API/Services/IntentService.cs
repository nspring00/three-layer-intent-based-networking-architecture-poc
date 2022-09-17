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

    public IList<Intent> GetIntents(string? regionFilter)
    {
        if (string.IsNullOrWhiteSpace(regionFilter))
        {
            return _intentRepository.GetAll();
        }

        return _intentRepository.GetForRegion(new Region(regionFilter));
    }

    public Intent? AddIntent(Intent intent)
    {
        return _intentRepository.Add(intent);
    }

    public IDictionary<KeyPerformanceIndicator, MinMaxTarget> GetKpiTargetsForRegion(Region region)
    {
        return _intentRepository.GetForRegion(region)
            .GroupBy(x => x.Target.Kpi)
            .ToDictionary(x => x.Key, x =>
            {
                var min = x.FirstOrDefault(k => k.Target.TargetMode == TargetMode.Min)?.Target.TargetValue;
                var max = x.FirstOrDefault(k => k.Target.TargetMode == TargetMode.Max)?.Target.TargetValue;
                return new MinMaxTarget(min, max);
            });
    }
}
