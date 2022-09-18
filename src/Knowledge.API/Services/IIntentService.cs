using Common.Models;
using Knowledge.API.Models;

namespace Knowledge.API.Services;

public interface IIntentService
{
    IList<Intent> GetIntents(string? regionFilter);
    Intent? GetIntentById(int id);
    Intent? AddIntent(Intent intent);
    IDictionary<KeyPerformanceIndicator, MinMaxTarget> GetKpiTargetsForRegion(Region region);
    bool RemoveIntent(int id);
    bool UpdateIntent(Intent intent);
}
