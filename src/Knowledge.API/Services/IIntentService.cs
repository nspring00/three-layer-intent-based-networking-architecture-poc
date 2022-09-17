using Common.Models;
using Knowledge.API.Models;

namespace Knowledge.API.Services;

public interface IIntentService
{
    Intent? AddIntent(Intent intent);
    IDictionary<KeyPerformanceIndicator, MinMaxTarget> GetKpiTargetsForRegion(Region region);
}
