using Knowledge.API.Models;
using Knowledge.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Knowledge.API.Controllers;

[ApiController]
[Route("intents")]
public class BusinessIntentController : ControllerBase
{
    private readonly ILogger<BusinessIntentController> _logger;
    private readonly IReasoningService _reasoningService;

    public BusinessIntentController(
        ILogger<BusinessIntentController> logger,
        IReasoningService reasoningService)
    {
        _logger = logger;
        _reasoningService = reasoningService;
    }

    // TODO move to message queue listener
    [HttpPost]
    public ActionResult<List<ReasoningComposition>> RequestReasoning([FromBody] List<string> regions)
    {
        return regions.Select(r => _reasoningService.ReasonForRegion(r)).ToList();
    }

}
