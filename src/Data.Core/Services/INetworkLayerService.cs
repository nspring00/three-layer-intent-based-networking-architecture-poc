using Common.Models;

namespace Data.Core.Services;

public interface INetworkLayerService
{
    Task FetchAllUpdates(int nlId, Region region, Uri uri);
}
