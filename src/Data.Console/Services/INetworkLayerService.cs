using Common.Models;
using Data.Console.Models;

namespace Data.Console.Services;

public interface INetworkLayerService
{
    Task FetchAllUpdates(Region region, Uri uri);
}
