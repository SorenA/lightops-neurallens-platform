using Microsoft.AspNetCore.Mvc;

namespace LightOps.NeuralLens.ObservabilityApi.Controllers;

[ApiController]
[Route("traces")]
public class TraceController(ILogger<TraceController> logger) : ControllerBase
{
    [HttpGet("", Name = "GetTraces")]
    public void GetTraces()
    {
        logger.LogInformation("GetTraces called");
    }
}
