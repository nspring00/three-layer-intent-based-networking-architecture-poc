using Common.Models;
using Knowledge.API.Models;

namespace Knowledge.API.Repository;

public interface IIntentRepository
{
    IList<KpiTarget> GetForRegion(Region region);
}
