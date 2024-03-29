﻿using NetworkLayer.API.Models;
using NetworkLayer.API.Repositories;

namespace NetworkLayer.API.Services;

public class NetworkObjectService : INetworkObjectService
{
    private readonly INetworkObjectRepository _networkObjectRepository;

    public NetworkObjectService(INetworkObjectRepository networkObjectRepository)
    {
        _networkObjectRepository = networkObjectRepository;
    }

    public IList<NetworkObject> GetAll()
    {
        return _networkObjectRepository.GetAll();
    }

    public (IList<NetworkObject>, IList<(NetworkObject, DateTime)>) GetChanges()
    {
        var created = _networkObjectRepository.GetCreated();
        var removed = _networkObjectRepository.GetRemoved();
        _networkObjectRepository.ResetCreateDelete();
        return (created, removed);
    }
}
