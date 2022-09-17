using Common.Models;
using Knowledge.API.Models;

namespace Knowledge.API.Repository;

public class CachedIntentRepository : IIntentRepository
{
    private readonly ILogger<CachedIntentRepository> _logger;
    private readonly List<Intent> _intents = new();
    private readonly IdGenerator _idGenerator = new();

    public CachedIntentRepository(ILogger<CachedIntentRepository> logger)
    {
        _logger = logger;
        // TODO remove after testing
        _intents.Add(
            new Intent(
                new Region("Vienna"), new KpiTarget(KeyPerformanceIndicator.Efficiency, TargetMode.Max, 0.7f))
            {
                Id = _idGenerator.Next()
            }
        );
        _intents.Add(
            new Intent(
                new Region("Linz"), new KpiTarget(KeyPerformanceIndicator.Efficiency, TargetMode.Min, 0.8f))
            {
                Id = _idGenerator.Next()
            }
        );
    }

    public IList<Intent> GetAll()
    {
        _logger.LogInformation("Retrieving all {Count} intents", _intents.Count);
        return _intents;
    }

    public IList<Intent> GetForRegion(Region region)
    {
        _logger.LogInformation("Retrieving intents for region {Region}", region.Name);
        return _intents
            .Where(x => String.Equals(x.Region.Name, region.Name, StringComparison.CurrentCultureIgnoreCase))
            .ToList();
    }

    public Intent? GetById(int id)
    {
        return _intents.FirstOrDefault(x => x.Id == id);
    }

    public Intent? Add(Intent intent)
    {
        if (_intents.Any(x =>
                x.Region == intent.Region && x.Target.Kpi == intent.Target.Kpi &&
                x.Target.TargetMode == intent.Target.TargetMode))
        {
            _logger.LogError("Intent already exists for region {Region} and kpi {Kpi} and target mode {TargetMode}",
                intent.Region.Name, intent.Target.Kpi, intent.Target.TargetMode);
            return null;
        }

        intent.Id = _idGenerator.Next();
        _intents.Add(intent);

        _logger.LogInformation(
            "Added intent for region {Region} and kpi {Kpi} and target mode {TargetMode} with value {Value}",
            intent.Region.Name, intent.Target.Kpi, intent.Target.TargetMode, intent.Target.TargetValue);
        return intent;
    }

    public bool Remove(int id)
    {
        _logger.LogInformation("Removing intent with id {Id}", id);
        return _intents.RemoveAll(x => x.Id == id) > 0;
    }

    private class IdGenerator
    {
        private int _nextId = 1;
        public int Next() => _nextId++;
    }
}
