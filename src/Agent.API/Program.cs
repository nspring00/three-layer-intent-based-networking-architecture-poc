using Agent.API;
using Common.Web.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.CheckEnforceHttp2();
builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "Hello from Agent");

app.Run();
