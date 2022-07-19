using Grpc.Core;
using NetworkLayer.API.Repositories;
using NetworkLayer.Grpc.Topology;

namespace NetworkLayer.API.Services;

public class NetworkTopologyUpdateService : NetworkTopologyUpdater.NetworkTopologyUpdaterBase
{
    private readonly ILogger<NetworkTopologyUpdateService> _logger;
    private readonly INetworkObjectRepository _networkObjectRepository;

    public NetworkTopologyUpdateService(ILogger<NetworkTopologyUpdateService> logger,
        INetworkObjectRepository networkObjectRepository)
    {
        _logger = logger;
        _networkObjectRepository = networkObjectRepository;
    }

    public override Task<ScaleUpResponse> ScaleUp(ScaleUpRequest request, ServerCallContext context)
    {
        var newNOs = request.NewNetworkObjects.Select(x => new Models.NetworkObject
        {
            Application = x.Application, Groups = x.Groups.ToList(), Ip = "TODO"
        }).ToList();


        var ids = new List<int>();
        foreach (var no in newNOs)
        {
            _networkObjectRepository.Create(no);
            ids.Add(no.Id);
        }

        _logger.LogInformation("Scaling up by {ScaleUpCount}. Ids {Ids}", newNOs.Count,
            string.Join(", ", ids));


        var response = new ScaleUpResponse
        {
            CreatedIds =
            {
                ids
            }
        };
        return Task.FromResult(response);
    }

    public override Task<ScaleDownResponse> ScaleDown(ScaleDownRequest request, ServerCallContext context)
    {
        var idsNotFound = new List<int>();
        foreach (var removeId in request.RemoveIds)
        {
            var success = _networkObjectRepository.Delete(removeId);
            if (!success)
            {
                idsNotFound.Add(removeId);
            }
        }

        _logger.LogInformation("Scaling down by {ScaleDownCount}. Removed {RemovedIds}. Not found {NotFoundIds}",
            request.RemoveIds.Count,
            string.Join(", ", request.RemoveIds.Except(idsNotFound)),
            string.Join(", ", idsNotFound));

        var response = new ScaleDownResponse
        {
            NotFoundIds =
            {
                idsNotFound
            }
        };
        return Task.FromResult(response);
    }
}
