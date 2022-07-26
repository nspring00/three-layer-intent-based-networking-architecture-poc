using Agent.Core.Clients;

namespace Agent.Core.Models;

public record NlManagerTopology(int Id, Uri Uri, IList<NetworkDevice> Devices);
