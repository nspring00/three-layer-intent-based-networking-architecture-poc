using FastEndpoints;
using Knowledge.API.Contracts.Requests;
using Knowledge.API.Contracts.Responses;
using Knowledge.API.Mappers;
using Knowledge.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace Knowledge.API.Endpoints.Intents;

[AllowAnonymous]
[HttpGet("/intents/{id}")]
public class GetIntentByIdEndpoint : Endpoint<GetIntentByIdRequest, GetIntentResponse, GetIntentMapper>
{
    private readonly IIntentService _intentService;

    public GetIntentByIdEndpoint(IIntentService intentService)
    {
        _intentService = intentService;
    }
    
    public override async Task HandleAsync(GetIntentByIdRequest req, CancellationToken ct)
    {
        var result = _intentService.GetIntentById(req.Id);
        if (result is null)
        {
            await SendNotFoundAsync();
            return;
        }
        
        await SendOkAsync(Map.FromEntity(result));
    }
}
