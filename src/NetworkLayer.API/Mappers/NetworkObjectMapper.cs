using NetworkLayer.API.Models;

namespace NetworkLayer.API.Mappers;

public static class NetworkObjectMapper
{
    public static Grpc.NetworkObjects.NetworkObject MapNetworkObjectToGrpc(NetworkObject no)
    {
        return new Grpc.NetworkObjects.NetworkObject
        {
            Id = no.Id,
            Utilization = MapUtilizationToGrpc(no.Utilization),
            Availability = no.Availability
        };
    }

    private static Grpc.NetworkObjects.Utilization MapUtilizationToGrpc(Utilization util)
    {
        return new Grpc.NetworkObjects.Utilization
        {
            CpuUsage = util.CpuUtilization,
            MemoryUsage = util.MemoryUtilization
        };
    }
}
