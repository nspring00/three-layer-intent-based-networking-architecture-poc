﻿using Common.Services;
using Common.Web.AspNetCore;
using NetworkLayer.API.Options;
using NetworkLayer.API.Repositories;
using NetworkLayer.API.Services;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDocker())
{
    builder.ConfigurePortsForRestAndGrpcNoTls();
}

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.Services.Configure<List<ExistingNoConfig>>(builder.Configuration.GetSection("NetworkObjects"));

builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddSingleton<INetworkObjectRepository, NetworkObjectRepository>();
builder.Services.AddSingleton<INetworkObjectService, NetworkObjectService>();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsDocker())
{
    app.MapGrpcReflectionService();
}

// Configure the HTTP request pipeline.
app.MapGet("/", () => "Hello from NetworkLayer");
app.MapGrpcService<NetworkObjectUpdateService>();
app.MapGrpcService<NetworkTopologyUpdateService>();

app.Run();