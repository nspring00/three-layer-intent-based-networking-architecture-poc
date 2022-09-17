using FastEndpoints;
using Knowledge.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace Knowledge.API.Endpoints.Intents;

[AllowAnonymous]
[HttpDelete("/intents/{id:int}")]
// TODO investigate why this is not working
public class RemoveIntentEndpoint : Endpoint<RemoveIntentRequest, EmptyResponse>
{
    private readonly IIntentService _intentService;

    // public override void Configure()
    // {
    //     AllowAnonymous();
    //     // Delete("/intents/{id}");
    // }

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

public class RemoveIntentRequest // : Knowledge.API.Contracts.Requests.RemoveIntentRequest
{
    [Microsoft.AspNetCore.Mvc.FromRoute]
    public new int Id { get; set; }
}
