using Grpc.Core;
using NetworkLayer.API.Repositories;
using NetworkLayer.Grpc.Topology;

namespace NetworkLayer.API.Services;

public class NetworkTopologyUpdateService : NetworkTopologyUpdater.NetworkTopologyUpdaterBase
{
    private readonly INetworkObjectRepository _networkObjectRepository;

    public NetworkTopologyUpdateService(INetworkObjectRepository networkObjectRepository)
    {
        _networkObjectRepository = networkObjectRepository;
    }

    public override Task<ScaleUpResponse> ScaleUp(ScaleUpRequest request, ServerCallContext context)
    {
        var newNOs = request.NewNetworkObjects.Select(x => new Models.NetworkObject
        {
           Id = x.Id,
           Application = x.Application,
           Groups = x.Groups.ToList(),
           Ip = "TODO"
        });

        foreach (var no in newNOs)
        {
            _networkObjectRepository.Create(no);
        }

        var response = new ScaleUpResponse();
        return Task.FromResult(response);
    }

    public override Task<ScaleDownResponse> ScaleDown(ScaleDownRequest request, ServerCallContext context)
    {
        foreach (var removeId in request.RemoveIds)
        {
            _networkObjectRepository.Delete(removeId);
        }

        var response = new ScaleDownResponse();
        return Task.FromResult(response);
    }
}
