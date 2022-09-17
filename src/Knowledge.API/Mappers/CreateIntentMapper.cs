using Common.Models;
using FastEndpoints;
using Knowledge.API.Contracts.Requests;
using Knowledge.API.Contracts.Responses;
using Knowledge.API.Models;

namespace Knowledge.API.Mappers;

public class CreateIntentMapper : Mapper<CreateIntentRequest, CreateIntentResponse, Intent>
{
    public override Intent ToEntity(CreateIntentRequest r)
    {
        return new Intent(
            new Region(r.Region),
            new KpiTarget(
                Enum.Parse<KeyPerformanceIndicator>(r.Kpi, true),
                Enum.Parse<TargetMode>(r.TargetMode, true),
                r.Value)
        );
    }

    public override CreateIntentResponse FromEntity(Intent e)
    {
        return new CreateIntentResponse
        {
            Id = e.Id
        };
    }
}
