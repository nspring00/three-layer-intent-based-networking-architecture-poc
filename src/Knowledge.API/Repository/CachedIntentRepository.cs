using Common.Models;
using Knowledge.API.Models;

namespace Knowledge.API.Repository;

public class CachedIntentRepository : IIntentRepository
{
    private readonly ILogger<CachedIntentRepository> _logger;
    private readonly Dictionary<Region, List<Intent>> _intents = new();

    public CachedIntentRepository(ILogger<CachedIntentRepository> logger)
    {
        _logger = logger;
        // TODO remove after testing
        _intents.Add(new Region("Vienna"),
            new List<Intent>
            {
                new(
                    new Region("Vienna"), new KpiTarget(KeyPerformanceIndicator.Efficiency, TargetMode.Max, 0.7f))
            });
        _intents.Add(new Region("Linz"),
            new List<Intent>
            {
                new(
                    new Region("Linz"), new KpiTarget(KeyPerformanceIndicator.Efficiency, TargetMode.Min, 0.8f))
            });
    }

    public Intent? Add(Intent intent)
    {
        if (_intents.ContainsKey(intent.Region))
        {
            if (_intents[intent.Region].Any(x =>
                    x.Target.Kpi == intent.Target.Kpi && x.Target.TargetMode == intent.Target.TargetMode))
            {
                _logger.LogError("Intent already exists for region {Region} and kpi {Kpi} and target mode {TargetMode}",
                    intent.Region.Name, intent.Target.Kpi, intent.Target.TargetMode);
                return null;
            }

            _intents[intent.Region].Add(intent);
        }
        else
        {
            _intents.Add(intent.Region, new List<Intent>
            {
                intent
            });
        }

        _logger.LogInformation(
            "Added intent for region {Region} and kpi {Kpi} and target mode {TargetMode} with value {Value}",
            intent.Region.Name, intent.Target.Kpi, intent.Target.TargetMode, intent.Target.TargetValue);
        return intent;
    }

    public IList<Intent> GetForRegion(Region region)
    {
        return _intents[region];
    }
}
