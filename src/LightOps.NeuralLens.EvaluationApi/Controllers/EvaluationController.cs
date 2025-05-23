using Microsoft.AspNetCore.Mvc;

namespace LightOps.NeuralLens.EvaluationApi.Controllers;

[ApiController]
[Route("evaluations")]
public class EvaluationController(ILogger<EvaluationController> logger) : ControllerBase
{
    [HttpGet("", Name = "GetEvaluations")]
    public void GetEvaluations()
    {
        logger.LogInformation("GetEvaluations called");
    }
}
