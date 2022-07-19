using Common.Web.AspNetCore;
using Data.API;
using Data.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.CheckEnforceHttp2();
builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

//app.UseHttpsRedirection();

app.MapGet("/", () => "Hello from Data");

app.MapGrpcService<TopologyService>();

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.Run();
