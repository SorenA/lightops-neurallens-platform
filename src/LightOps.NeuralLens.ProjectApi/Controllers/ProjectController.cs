using Microsoft.AspNetCore.Mvc;

namespace LightOps.NeuralLens.ProjectApi.Controllers;

[ApiController]
[Route("projects")]
public class ProjectController(ILogger<ProjectController> logger) : ControllerBase
{
    [HttpGet("", Name = "GetProjects")]
    public void GetProjects()
    {
        logger.LogInformation("GetProjects called");
    }
}
