using Common.Web.AspNetCore;
using FastEndpoints;
using FastEndpoints.Swagger;
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
builder.Services.AddSwaggerDoc();

// Registration of custom services
builder.Services.ConfigureServices(builder.Configuration, builder.Environment);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsDocker())
{
    app.UseOpenApi();
    app.UseSwaggerUi3(c => c.ConfigureDefaults());

    app.MapGrpcReflectionService();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.UseDefaultExceptionHandler();
app.MapGet("/", () => "Hello from Knowledge");
// TODO find solution for fast endpoints error
app.MapDelete("/intents/{id:int}",
    (int id, IIntentService intentService) => intentService.RemoveIntent(id) ? Results.Ok() : Results.NotFound());
app.UseFastEndpoints();
app.MapGrpcServices();

app.Run();
