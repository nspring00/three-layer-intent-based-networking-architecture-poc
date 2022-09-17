using FastEndpoints;
using Knowledge.API.Contracts.Requests;
using Knowledge.API.Contracts.Responses;
using Knowledge.API.Models;

namespace Knowledge.API.Mappers;

public class GetIntentsMapper : Mapper<GetIntentsRequest, List<GetIntentResponse>,
    IList<Intent>>
{
    public override List<GetIntentResponse> FromEntity(IList<Intent> intents)
    {
        return intents.Select(MapIntentToGetIntentResponse).ToList();
    }

    private static GetIntentResponse MapIntentToGetIntentResponse(Intent intent)
    {
        return new GetIntentResponse
        {
            Id = intent.Id,
            Region = intent.Region.Name,
            Kpi = intent.Target.Kpi.ToString(),
            TargetMode = intent.Target.TargetMode.ToString(),
            Value = intent.Target.TargetValue
        };
    }
}
