using Knowledge.API.Models;

namespace Knowledge.API.Repository;

public class IntentRepository
{
    private readonly List<Intent> _intents = new();

    public IntentRepository()
    {
        // TODO remove after testing
        _intents.Add(new Intent(
            new Region("Vienna"), new Efficiency(TargetMode.Max, 0.9f)));
    }

    public void Add(Intent intent)
    {
        _intents.Add(intent);
    }

    public IList<Intent> GetForRegion(string region)
    {
        return _intents.Where(i => i.At.Name == region).ToList();
    }
}
