using Common.Models;
using Knowledge.API.Models;

namespace Knowledge.API.Repository;

public class CachedIntentRepository : IIntentRepository
{
    private readonly List<Intent> _intents = new();

    public CachedIntentRepository()
    {
        // TODO remove after testing
        _intents.Add(new Intent(
            new Region("Vienna"), new Efficiency(TargetMode.Max, 0.7f)));
        _intents.Add(new Intent(
            new Region("Linz"), new Efficiency(TargetMode.Min, 0.8f)));
    }

    public void Add(Intent intent)
    {
        _intents.Add(intent);
    }

    public IList<Intent> GetForRegion(Region region)
    {
        return _intents.Where(i => i.At == region).ToList();
    }
}
