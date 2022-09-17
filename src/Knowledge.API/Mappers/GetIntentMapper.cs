using FastEndpoints;
using Knowledge.API.Contracts.Requests;
using Knowledge.API.Contracts.Responses;
using Knowledge.API.Models;

namespace Knowledge.API.Mappers;

public class GetIntentMapper : Mapper<GetIntentByIdRequest, GetIntentResponse, Intent>
{
    public override GetIntentResponse FromEntity(Intent intent)
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
