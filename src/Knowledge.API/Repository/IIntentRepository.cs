using Common.Models;
using Knowledge.API.Models;

namespace Knowledge.API.Repository;

public interface IIntentRepository
{
    Intent? Add(Intent intent);
    IList<Intent> GetForRegion(Region region);
}
