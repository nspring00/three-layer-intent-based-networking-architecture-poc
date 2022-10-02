using FastEndpoints;
using Knowledge.API.Contracts.Requests;
using Knowledge.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace Knowledge.API.Endpoints.Intents;

[AllowAnonymous]
[HttpDelete("intents/{id}")]
public class RemoveIntentEndpoint : Endpoint<RemoveIntentRequest>
{
    private readonly IIntentService _intentService;
    
    public RemoveIntentEndpoint(IIntentService intentService)
    {
        _intentService = intentService;
    }

    public override async Task HandleAsync(RemoveIntentRequest req, CancellationToken ct)
    {
        var success = _intentService.RemoveIntent(req.Id);
        if (!success)
        {
            await SendNotFoundAsync();
            return;
        }

        await SendOkAsync();
    }
}
