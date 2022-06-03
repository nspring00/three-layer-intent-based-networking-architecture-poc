using Knowledge.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Knowledge.API.Controllers;

[ApiController]
[Route("intents")]
public class BusinessIntentController : ControllerBase
{
    private readonly ILogger<BusinessIntentController> _logger;

    public BusinessIntentController(ILogger<BusinessIntentController> logger)
    {
        _logger = logger;
    }

    // TODO move to message queue listener
    [HttpPost]
    public IActionResult RequestReasoning([FromBody] List<string> regions)
    {
        throw new NotImplementedException();
    }

}
