using NetworkLayer.API.Models;

namespace NetworkLayer.API.Mappers;

public static class NetworkObjectMapper
{
    public static Grpc.NetworkObject MapNetworkObjectToGrpc(NetworkObject no)
    {
        return new Grpc.NetworkObject
        {
            Id = no.Id,
            Utilization = MapUtilizationToGrpc(no.Utilization),
            Availability = no.Availability
        };
    }

    private static Grpc.Utilization MapUtilizationToGrpc(Utilization util)
    {
        return new Grpc.Utilization
        {
            CpuUsage = util.CpuUtilization,
            MemoryUsage = util.MemoryUtilization
        };
    }
}
