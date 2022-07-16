using Common.Models;
using Common.Services;
using Data.Console.Models;
using Data.Console.Repositories;
using Data.Grpc.Topology;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Data.API.Services;

public class TopologyService : Grpc.Topology.TopologyService.TopologyServiceBase
{
    private readonly ILogger<TopologyService> _logger;
    private readonly INetworkObjectRepository _networkObjectRepository;
    private readonly INlManagerService _nlManagerService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public TopologyService(
        ILogger<TopologyService> logger,
        INetworkObjectRepository networkObjectRepository,
        INlManagerService nlManagerService,
        IDateTimeProvider dateTimeProvider)
    {
        _logger = logger;
        _networkObjectRepository = networkObjectRepository;
        _nlManagerService = nlManagerService;
        _dateTimeProvider = dateTimeProvider;
    }

    public override Task<RegionTopologyResponse> GetTopologyForRegions(
        RegionTopologyRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("Topologies requested for regions {Regions}", string.Join(", ", request.RegionNames));

        var now = _dateTimeProvider.Now;
        var topologies = request.RegionNames
            .Select(x => new Region(x))
            .Select(r => (Region: r, Devices: _networkObjectRepository.GetAllForRegionByNlId(r)))
            .Where(x => x.Devices is not null)
            .Select(x => MapRegionTopology(x.Region, x.Devices!, now))
            .ToList();

        _logger.LogInformation("Found {NlManagerCount} NlManagers with {DeviceCount} devices for regions {Regions}",
            topologies.Sum(x => x.NlManagers.Count),
            topologies.Sum(x => x.NlManagers.Sum(y => y.ActiveDevices.Count)),
            string.Join(", ", request.RegionNames));

        var response = new RegionTopologyResponse { RegionTopologies = { topologies } };
        return Task.FromResult(response);
    }

    private RegionTopology MapRegionTopology(
        Region r,
        IDictionary<int, IList<NetworkObject>> deviceDict,
        DateTime now)
    {
        return new RegionTopology
        {
            RegionName = r.Name, NlManagers = { deviceDict.Select(x => MapNlManager(x.Key, x.Value, now)) }
        };
    }

    private NlManager MapNlManager(int nlId, IEnumerable<NetworkObject> devices, DateTime now)
    {
        return new NlManager
        {
            Id = nlId,
            Uri = GetUriForNlManager(nlId)?.AbsoluteUri,
            ActiveDevices = { devices.Select(x => MapDevice(x, now)) }
        };
    }

    private static DeviceInfo MapDevice(NetworkObject no, DateTime now)
    {
        return new DeviceInfo { Id = no.Id, Uptime = Duration.FromTimeSpan(now - no.Created) };
    }

    private Uri? GetUriForNlManager(int nlId)
    {
        return _nlManagerService.GetUriById(nlId);
    }
}
