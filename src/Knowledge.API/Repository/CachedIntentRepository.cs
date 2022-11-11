using Common.Models;
using Knowledge.API.Configs;
using Knowledge.API.Models;
using Microsoft.Extensions.Options;

namespace Knowledge.API.Repository;

public class CachedIntentRepository : IIntentRepository
{
    private readonly ILogger<CachedIntentRepository> _logger;
    private readonly List<Intent> _intents = new();
    private readonly IdGenerator _idGenerator = new();

    public CachedIntentRepository(ILogger<CachedIntentRepository> logger, IOptions<List<InitialIntent>> initialIntents)
    {
        _logger = logger;

        if (initialIntents.Value.Count == 0)
        {
            _logger.LogInformation("No initial intents found");
            return;
        }
        
        _intents = initialIntents.Value.Select(x => new Intent(
            new Region(x.Region),
            new KpiTarget(
                Enum.Parse<KeyPerformanceIndicator>(x.Kpi, true),
                Enum.Parse<TargetMode>(x.TargetMode, true),
                x.TargetValue
            )
        )
        {
            Id = _idGenerator.Next(),
        }).ToList();
        
        _logger.LogInformation("Loaded {Count} initial intents", _intents.Count);
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

        switch (intent.Target.TargetMode)
        {
            // Check there is no max < min for the same region and kpi
            case TargetMode.Min when _intents.Any(x =>
                x.Region == intent.Region && x.Target.Kpi == intent.Target.Kpi &&
                x.Target.TargetMode == TargetMode.Max && x.Target.TargetValue < intent.Target.TargetValue):
                _logger.LogError(
                    "Intent with target mode {TargetMode} is not allowed because there is already a max intent with a lower value for region {Region} and kpi {Kpi}",
                    intent.Target.TargetMode, intent.Region.Name, intent.Target.Kpi);
                return null;
            // Check there is no min > max for the same region and kpi
            case TargetMode.Max when _intents.Any(x =>
                x.Region == intent.Region && x.Target.Kpi == intent.Target.Kpi &&
                x.Target.TargetMode == TargetMode.Min && x.Target.TargetValue > intent.Target.TargetValue):
                _logger.LogError(
                    "Intent with target mode {TargetMode} is not allowed because there is already a min intent with a higher value for region {Region} and kpi {Kpi}",
                    intent.Target.TargetMode, intent.Region.Name, intent.Target.Kpi);
                return null;
        }

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

    public bool Update(Intent intent)
    {
        _logger.LogInformation("Updating intent with id {Id}", intent.Id);
        if (_intents.All(x => x.Id != intent.Id))
        {
            _logger.LogError("Intent with id {Id} does not exist", intent.Id);
            return false;
        }

        if (_intents.Any(x => x.Region == intent.Region && x.Target.Kpi == intent.Target.Kpi &&
                              x.Target.TargetMode == intent.Target.TargetMode && x.Id != intent.Id))
        {
            _logger.LogError("Intent already exists for region {Region} and kpi {Kpi} and target mode {TargetMode}",
                intent.Region.Name, intent.Target.Kpi, intent.Target.TargetMode);
            return false;
        }

        switch (intent.Target.TargetMode)
        {
            // Check there is no max < min for the same region and kpi
            case TargetMode.Min when _intents.Any(x =>
                x.Id != intent.Id && x.Region == intent.Region && x.Target.Kpi == intent.Target.Kpi &&
                x.Target.TargetMode == TargetMode.Max && x.Target.TargetValue < intent.Target.TargetValue):
                _logger.LogError(
                    "Intent with target mode {TargetMode} is not allowed because there is already a max intent with a lower value for region {Region} and kpi {Kpi}",
                    intent.Target.TargetMode, intent.Region.Name, intent.Target.Kpi);
                return false;
            // Check there is no min > max for the same region and kpi
            case TargetMode.Max when _intents.Any(x =>
                x.Id != intent.Id && x.Region == intent.Region && x.Target.Kpi == intent.Target.Kpi &&
                x.Target.TargetMode == TargetMode.Min && x.Target.TargetValue > intent.Target.TargetValue):
                _logger.LogError(
                    "Intent with target mode {TargetMode} is not allowed because there is already a min intent with a higher value for region {Region} and kpi {Kpi}",
                    intent.Target.TargetMode, intent.Region.Name, intent.Target.Kpi);
                return false;
        }

        var existingIntent = _intents.First(x => x.Id == intent.Id);
        _intents.Remove(existingIntent);
        _intents.Add(intent);
        return true;
    }

    private class IdGenerator
    {
        private int _nextId = 1;
        public int Next() => _nextId++;
    }
}
