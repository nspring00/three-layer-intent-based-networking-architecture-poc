using FastEndpoints;
using Knowledge.API.Contracts.Requests;
using Knowledge.API.Contracts.Responses;
using Knowledge.API.Models;

namespace Knowledge.API.Mappers;

public class GetIntentsMapper : Mapper<GetIntentsRequest, GetIntentsResponse, IList<Intent>>
{
    public override GetIntentsResponse FromEntity(IList<Intent> e)
    {
        return new GetIntentsResponse
        {
            Intents = e.Select(x => new GetIntentResponse
            {
                Region = x.Region.Name,
                Kpi = x.Target.Kpi.ToString(),
                TargetMode = x.Target.TargetMode.ToString(),
                Value = x.Target.TargetValue
            }).ToList()
        };
    }
}