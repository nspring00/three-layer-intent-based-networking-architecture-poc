using FastEndpoints;
using FastEndpoints.Swagger;
using Knowledge.Grpc.Reasoning;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder();
builder.Services.AddFastEndpoints();
builder.Services.AddSwaggerDoc();
builder.Services.AddGrpc();

var app = builder.Build();
app.UseAuthorization();
app.UseFastEndpoints();
app.UseOpenApi();
app.UseSwaggerUi3(c => c.ConfigureDefaults());
app.MapGrpcService<GrpcService>();
app.Run();

public class Request
{
    public int Id { get; set; }
}

[HttpGet("prefix/{id}")]
[AllowAnonymous]
public class GetEndpoint : Endpoint<Request>
{
    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        await SendAsync(req.Id);
    }
}

[HttpDelete("prefix/{id}")]
[AllowAnonymous]
// not working
public class Endpoint : Endpoint<Request>
{
    public override async Task HandleAsync(Request r, CancellationToken c)
    {
        await SendAsync(r.Id);
    }
}

public class GrpcService : ReasoningService.ReasoningServiceBase
{
    
}
