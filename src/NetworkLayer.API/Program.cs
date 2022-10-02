using Common.Services;
using Common.Web.AspNetCore;
using NetworkLayer.API.Options;
using NetworkLayer.API.Repositories;
using NetworkLayer.API.Services;
using NetworkLayer.API.Simulation;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDocker() || builder.Environment.IsEcs())
{
    builder.ConfigurePortsForRestAndGrpcNoTls();
}

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.Services.Configure<SimulationConfig>(builder.Configuration.GetSection("Simulation"));

builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddSingleton<INetworkObjectRepository, SimulationNetworkObjectRepository>();
builder.Services.AddSingleton<SimulationDataSet>();
builder.Services.AddSingleton<INetworkObjectService, NetworkObjectService>();

var app = builder.Build();

app.MapGrpcReflectionService();

// Configure the HTTP request pipeline.
app.MapGet("/", () => "Hello from NetworkLayer");
app.MapGrpcService<NetworkObjectUpdateService>();
app.MapGrpcService<NetworkTopologyUpdateService>();

app.Run();
