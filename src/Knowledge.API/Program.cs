using Common.Web.AspNetCore;
using FastEndpoints;
using Knowledge.API;
using Knowledge.API.Services;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDocker())
{
    builder.ConfigurePortsForRestAndGrpcNoTls();
}

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddFastEndpoints();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registration of custom services
builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsDocker())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapGrpcReflectionService();
}

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapGet("/", () => "Hello from Knowledge");
// TODO for some reason fastEndpoints delete does not bind
app.MapDelete("/intents/{id:int}",
    (int id, IIntentService intentService) => intentService.RemoveIntent(id) ? Results.Ok() : Results.NotFound());
app.UseFastEndpoints();
app.MapGrpcServices();

app.Run();
