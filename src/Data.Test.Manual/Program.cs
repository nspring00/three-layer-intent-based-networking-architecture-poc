using Data.Grpc.Topology;
using Grpc.Net.Client;

var dataGrpcUri = new Uri("https://localhost:7110");

var channel = GrpcChannel.ForAddress(dataGrpcUri);

var client = new TopologyService.TopologyServiceClient(channel);

var request = new RegionTopologyRequest
{
    RegionNames =
    {
        "Vienna",
        "Linz",
        "Not found..."
    }
};

var response = await client.GetTopologyForRegionsAsync(request);

Console.WriteLine(response);

Console.ReadLine();
