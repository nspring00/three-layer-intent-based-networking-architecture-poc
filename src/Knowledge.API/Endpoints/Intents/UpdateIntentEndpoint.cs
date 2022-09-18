using Common.Models;
using FastEndpoints;
using Knowledge.API.Models;
using Knowledge.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace Knowledge.API.Endpoints.Intents;

[AllowAnonymous]
[HttpPut("/intents/{id}")]
public class UpdateIntentEndpoint : Endpoint<UpdateIntentRequest, EmptyResponse, UpdateIntentMapper>
{
    private readonly IIntentService _intentService;

    public UpdateIntentEndpoint(IIntentService intentService)
    {
        _intentService = intentService;
    }
    
    public override async Task HandleAsync(UpdateIntentRequest req, CancellationToken ct)
    {
        var success = _intentService.UpdateIntent(Map.ToEntity(req));
        if (!success)
        {
            await SendErrorsAsync();
            return;
        }

        await SendOkAsync();
    }
}

public class UpdateIntentMapper : Mapper<UpdateIntentRequest, EmptyResponse, Intent>
{
    public override Intent ToEntity(UpdateIntentRequest r)
    {
        return new Intent(
            new Region(r.Region),
            new KpiTarget(
                Enum.Parse<KeyPerformanceIndicator>(r.Kpi, true),
                Enum.Parse<TargetMode>(r.TargetMode, true),
                r.Value)
        )
        {
            Id = r.Id
        };
    }
}

public class UpdateIntentRequest
{
    public int Id { get; set; }
    public string Region { get; set; } = default!;
    public string Kpi { get; set; } = default!;
    public string TargetMode { get; set; } = default!;
    public float Value { get; set; }
}
