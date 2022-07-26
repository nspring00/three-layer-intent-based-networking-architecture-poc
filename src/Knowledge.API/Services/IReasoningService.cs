using Common.Models;
using Knowledge.API.Models;

namespace Knowledge.API.Services;

public interface IReasoningService
{
    ReasoningComposition ReasonForRegion(Region region);
}
