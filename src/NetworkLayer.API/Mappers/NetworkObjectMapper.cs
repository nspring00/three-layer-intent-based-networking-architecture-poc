using NetworkLayer.API.Models;

namespace NetworkLayer.API.Mappers;

public static class NetworkObjectMapper
{
    public static Protos.NetworkObject MapNetworkObjectToGrpc(NetworkObject no)
    {
        return new Protos.NetworkObject
        {
            Id = no.Id,
            Application = no.Application,
            Utilization = MapUtilizationToGrpc(no.Utilization),
            Availability = no.Availability
        };
    }

    private static Protos.Utilization MapUtilizationToGrpc(Utilization util)
    {
        return new Protos.Utilization
        {
            CpuUsage = util.CpuUtilization,
            MemoryUsage = util.MemoryUtilization
        };
    }
}
