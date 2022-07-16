using Common.Services;
using NetworkLayer.API.Repositories;
using NetworkLayer.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddSingleton<INetworkObjectRepository, NetworkObjectRepository>();
builder.Services.AddSingleton<INetworkObjectService, NetworkObjectService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

// Configure the HTTP request pipeline.
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
app.MapGrpcService<NetworkObjectUpdateService>();

app.Run();
