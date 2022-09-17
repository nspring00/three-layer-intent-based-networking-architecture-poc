using FastEndpoints;
using Knowledge.API.Contracts.Requests;
using Knowledge.API.Contracts.Responses;
using Knowledge.API.Mappers;
using Knowledge.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace Knowledge.API.Endpoints.Intents;

[AllowAnonymous]
[HttpGet("/intents")]
public class GetIntentsEndpoint : Endpoint<GetIntentsRequest, List<GetIntentResponse>, GetIntentsMapper>
{
    private readonly IIntentService _intentService;

    public GetIntentsEndpoint(IIntentService intentService)
    {
        _intentService = intentService;
    }
    
    public override  Task HandleAsync(GetIntentsRequest req, CancellationToken ct)
    {
        var intents = _intentService.GetIntents(req.Region);
        Response = Map.FromEntity(intents);
        return Task.CompletedTask;
    }
}
