using Common.Models;
using MediatR;

namespace Knowledge.API.Models;

public class WorkloadInfoAddedNotification : INotification
{
    public WorkloadInfoAddedNotification(DateTime timestamp, IList<Region> regions)
    {
        Timestamp = timestamp;
        Regions = regions;
    }

    public DateTime Timestamp { get; }
    public IList<Region> Regions { get; }
}
