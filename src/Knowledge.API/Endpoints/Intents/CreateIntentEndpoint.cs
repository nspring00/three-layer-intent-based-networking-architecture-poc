using FastEndpoints;
using Knowledge.API.Contracts.Requests;
using Knowledge.API.Contracts.Responses;
using Knowledge.API.Mappers;
using Knowledge.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace Knowledge.API.Endpoints.Intents;

[HttpPost("/intents")]
[AllowAnonymous]
public class CreateIntentEndpoint : Endpoint<CreateIntentRequest, CreateIntentResponse, CreateIntentMapper>
{
    private readonly ILogger<CreateIntentEndpoint> _logger;
    private readonly IIntentService _intentService;

    public CreateIntentEndpoint(ILogger<CreateIntentEndpoint> logger, IIntentService intentService)
    {
        _logger = logger;
        _intentService = intentService;
    }
    
    public override async Task HandleAsync(CreateIntentRequest req, CancellationToken ct)
    {
        var intent = Map.ToEntity(req);
     
        _logger.LogInformation("Add intent: {Intent}", intent);
        
        var result = _intentService.AddIntent(intent);
        if (result is null)
        {
            await SendErrorsAsync();
            return;
        }

        await SendAsync(Map.FromEntity(result));
    }

}
