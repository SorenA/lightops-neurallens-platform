using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace LightOps.NeuralLens.ObservabilityApi.Controllers;

[ApiVersion(1.0)]
[ApiController]
[Tags("Traces")]
[Route("v{version:apiVersion}/traces")]
public class TraceController(ILogger<TraceController> logger) : ControllerBase
{
    [HttpGet("", Name = "GetTraces")]
    public void GetTraces()
    {
        logger.LogInformation("GetTraces called");
    }
}
