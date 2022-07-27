using Common.Models;
using Knowledge.API.Models;
using Knowledge.API.Repository;
using Knowledge.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Knowledge.API.Controllers;

[ApiController]
[Route("intents")]
public class BusinessIntentController : ControllerBase
{
    private readonly ILogger<BusinessIntentController> _logger;
    private readonly IReasoningService _reasoningService;
    private readonly IWorkloadRepository _workloadRepository;

    public BusinessIntentController(
        ILogger<BusinessIntentController> logger,
        IReasoningService reasoningService,
        IWorkloadRepository workloadRepository)
    {
        _logger = logger;
        _reasoningService = reasoningService;
        _workloadRepository = workloadRepository;
    }

    // TODO move to message queue listener
    [HttpPost]
    public ActionResult<List<ReasoningComposition>> RequestReasoning([FromBody] List<string> regions)
    {
        return regions.Select(r => _reasoningService.ReasonForRegion(new Region(r))).ToList();
    }

    // TODO only for debugging
    [HttpGet("/devices/{region}")]
    public ActionResult<List<NetworkDevice>> GetDevices([FromRoute] string region)
    {
        return _workloadRepository.GetForRegion(new Region(region)).ToList();
    }

}
