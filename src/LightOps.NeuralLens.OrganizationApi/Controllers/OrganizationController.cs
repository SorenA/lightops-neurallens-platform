using Microsoft.AspNetCore.Mvc;

namespace LightOps.NeuralLens.OrganizationApi.Controllers;

[ApiController]
[Route("orgs")]
public class OrganizationController(ILogger<OrganizationController> logger) : ControllerBase
{
    [HttpGet("", Name = "GetOrganizations")]
    public void GetOrganizations()
    {
        logger.LogInformation("GetOrganizations called");
    }
}
