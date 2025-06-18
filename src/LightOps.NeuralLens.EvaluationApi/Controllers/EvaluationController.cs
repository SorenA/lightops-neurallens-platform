using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace LightOps.NeuralLens.EvaluationApi.Controllers;

[ApiVersion(1.0)]
[ApiController]
[Tags("Evaluations")]
[Route("v{version:apiVersion}/evaluations")]
public class EvaluationController(ILogger<EvaluationController> logger) : ControllerBase
{
    [HttpGet("", Name = "GetEvaluations")]
    public void GetEvaluations()
    {
        logger.LogInformation("GetEvaluations called");
    }
}
