using Cocona;
using Data.Console;
using Data.Console.Clients;
using Data.Console.Repositories;
using Data.Console.Services;
using Microsoft.Extensions.DependencyInjection;

var builder = CoconaApp.CreateBuilder();

builder.Services.AddSingleton<ServiceRunner>();
builder.Services.AddSingleton<INetworkObjectRepository, NetworkObjectRepository>();
builder.Services.AddSingleton<INetworkObjectService, NetworkObjectService>();
builder.Services.AddSingleton<INetworkLayerService, NetworkLayerService>();
builder.Services.AddSingleton<KnowledgeGrpcClient>();
builder.Services.AddSingleton<NlGrpcClient>();

var app = builder.Build();

app.AddCommand((ServiceRunner runner) => runner.Run());

app.Run();
