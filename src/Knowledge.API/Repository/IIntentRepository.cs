using Common.Models;
using Knowledge.API.Models;

namespace Knowledge.API.Repository;

public interface IIntentRepository
{
    IList<Intent> GetAll();
    Intent? Add(Intent intent);
    IList<Intent> GetForRegion(Region region);
}
