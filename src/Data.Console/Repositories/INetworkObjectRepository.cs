using Data.Console.Models;

namespace Data.Console.Repositories;
public interface INetworkObjectRepository
{
    void Create(NetworkObject networkObject);
    NetworkObject? Get(NOId id);
}
