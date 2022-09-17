using Common.Models;
using Knowledge.API.Models;

namespace Knowledge.API.Services;

public interface IIntentService
{
    IList<Intent> GetIntents(string? regionFilter);
    Intent? AddIntent(Intent intent);
    IDictionary<KeyPerformanceIndicator, MinMaxTarget> GetKpiTargetsForRegion(Region region);
}
