using Cocona;
using Data.Console;

var builder = CoconaApp.CreateBuilder();
builder.Services.ConfigureServices();

var app = builder.Build();

app.AddCommand((ServiceRunner runner) => runner.Run());

app.Run();
