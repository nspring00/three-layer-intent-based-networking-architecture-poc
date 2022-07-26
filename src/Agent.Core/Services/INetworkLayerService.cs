using Agent.Core.Models;

namespace Agent.Core.Services;

public interface INetworkLayerService
{
    Task<IList<int>> CreateNOsAsync(Uri uri, ICollection<NetworkObjectCreateInfo> newNOs);
    Task<IList<int>> DeleteNOsAsync(Uri uri, ICollection<int> removeIds);
}
