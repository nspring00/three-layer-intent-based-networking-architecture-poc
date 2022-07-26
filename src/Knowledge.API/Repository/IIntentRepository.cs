using Common.Models;
using Knowledge.API.Models;

namespace Knowledge.API.Repository;

public interface IIntentRepository
{
    IList<Intent> GetForRegion(Region region);
}
