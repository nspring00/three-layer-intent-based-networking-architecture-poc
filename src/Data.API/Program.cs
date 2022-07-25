using Common.Web.AspNetCore;
using Data.API;
using Data.API.Services;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDocker())
{
    builder.ConfigurePortsForRestAndGrpcNoTls();
}

builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

//app.UseHttpsRedirection();

app.MapGet("/", () => "Hello from Data");

app.MapGrpcService<TopologyService>();

if (app.Environment.IsDevelopment() || app.Environment.IsDocker())
{
    app.MapGrpcReflectionService();
}

app.Run();
