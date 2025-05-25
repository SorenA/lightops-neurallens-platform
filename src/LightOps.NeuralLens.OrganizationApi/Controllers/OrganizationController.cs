using LightOps.Mapping.Api.Services;
using LightOps.NeuralLens.OrganizationApi.Domain.Exceptions;
using LightOps.NeuralLens.OrganizationApi.Domain.Models;
using LightOps.NeuralLens.OrganizationApi.Domain.Services;
using LightOps.NeuralLens.OrganizationApi.Models;
using LightOps.NeuralLens.OrganizationApi.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LightOps.NeuralLens.OrganizationApi.Controllers;

[ApiController]
[Route("organizations")]
public class OrganizationController(
    ILogger<OrganizationController> logger,
    IMappingService mappingService,
    OrganizationService organizationService)
    : ControllerBase
{
    [HttpGet("", Name = "GetOrganizations")]
    [ProducesResponseType<List<OrganizationViewModel>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetOrganizations()
    {
        logger.LogInformation("GetOrganizations called");
        var entities = await organizationService.GetAll();

        return Ok(mappingService.Map<Organization, OrganizationViewModel>(entities)
            .ToList());
    }

    [HttpGet("{id}", Name = "GetOrganizationById")]
    [ProducesResponseType<OrganizationViewModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetOrganizationById(string id)
    {
        logger.LogInformation("GetOrganizationById called");
        try
        {
            var entity = await organizationService.GetById(id);
            return Ok(mappingService.Map<Organization, OrganizationViewModel>(entity));
        }
        catch (OrganizationNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPatch("{id}", Name = "UpdateOrganization")]
    [ProducesResponseType<OrganizationViewModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateOrganization(string id, [FromBody] UpdateOrganizationRequest request)
    {
        logger.LogInformation("UpdateOrganization called");
        try
        {
            var entity = await organizationService.Update(id, request);
            return Ok(mappingService.Map<Organization, OrganizationViewModel>(entity));
        }
        catch (OrganizationNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}", Name = "DeleteOrganization")]
    [ProducesResponseType<OrganizationViewModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteOrganization(string id)
    {
        logger.LogInformation("DeleteOrganization called");
        try
        {
            var entity = await organizationService.Delete(id);
            return Ok(mappingService.Map<Organization, OrganizationViewModel>(entity));
        }
        catch (OrganizationNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("", Name = "CreateOrganization")]
    [ProducesResponseType<OrganizationViewModel>(StatusCodes.Status200OK)]
    public async Task<ActionResult> CreateOrganization([FromBody] CreateOrganizationRequest request)
    {
        logger.LogInformation("CreateOrganization called");
        var entity = await organizationService.Create(request);

        return Ok(mappingService.Map<Organization, OrganizationViewModel>(entity));
    }
}
