using Data.API.Models;

namespace Data.API.Services;

public interface INlManagerService
{
    Uri? GetUriById(int nlId);
    IList<NlManagerInfo> GetAll();
}
