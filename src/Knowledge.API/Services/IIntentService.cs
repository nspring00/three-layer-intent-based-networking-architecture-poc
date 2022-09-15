using Common.Models;
using Knowledge.API.Models;

namespace Knowledge.API.Services;

public interface IIntentService
{
    IDictionary<KeyPerformanceIndicator, MinMaxTarget> GetKpiTargetsForRegion(Region region);
}
