using Data.Grpc.Topology;
using Grpc.Core;

namespace Data.API.Services;

public class TopologyService : Grpc.Topology.TopologyService.TopologyServiceBase
{
    public override Task<RegionTopologyResponse> GetTopologyForRegions(RegionTopologyRequest request, ServerCallContext context)
    {


        return base.GetTopologyForRegions(request, context);
    }
}
