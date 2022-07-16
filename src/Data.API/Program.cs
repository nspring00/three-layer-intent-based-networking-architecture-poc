using Data.API;
using Data.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGrpcService<TopologyService>();

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.Run();
