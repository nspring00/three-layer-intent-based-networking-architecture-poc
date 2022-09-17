using Common.Models;
using Knowledge.API.Models;

namespace Knowledge.API.Repository;

public interface IIntentRepository
{
    IList<Intent> GetAll();
    IList<Intent> GetForRegion(Region region);
    Intent? GetById(int id);
    Intent? Add(Intent intent);
    bool Remove(int id);
}
