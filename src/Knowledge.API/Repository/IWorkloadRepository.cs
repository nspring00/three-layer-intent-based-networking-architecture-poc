using Common.Models;
using Knowledge.API.Models;

namespace Knowledge.API.Repository;

public interface IWorkloadRepository
{
    void Add(Region region, WorkloadInfo workloadInfo);
    IList<WorkloadInfo> GetForRegion(Region region);
    WorkloadInfo? GetLatest(Region region);

    //NetworkDevice? Get(Region region, int id);
    //IList<NetworkDevice> GetForRegion(Region region);
    //void Add(NetworkDevice networkDevice);
    //bool Remove(Region region, int id);
}
