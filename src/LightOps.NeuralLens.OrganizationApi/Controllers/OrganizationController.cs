using Asp.Versioning;
using LightOps.Mapping.Api.Services;
using LightOps.NeuralLens.Component.ServiceDefaults;
using LightOps.NeuralLens.OrganizationApi.Domain.Models;
using LightOps.NeuralLens.OrganizationApi.Domain.Services;
using LightOps.NeuralLens.OrganizationApi.Models;
using LightOps.NeuralLens.OrganizationApi.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LightOps.NeuralLens.OrganizationApi.Controllers;

[ApiVersion(1.0)]
[ApiController]
[Tags("Organizations")]
[Route("v{version:apiVersion}/organizations")]
[Authorize(Policy = AuthScopes.Organizations.Read)]
public class OrganizationController(
    IMappingService mappingService,
    OrganizationService organizationService)
    : ControllerBase
{
    [HttpGet("", Name = "GetOrganizations")]
    [ProducesResponseType<List<OrganizationViewModel>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetOrganizations()
    {
        var entities = await organizationService.GetAll();

        return Ok(mappingService.Map<Organization, OrganizationViewModel>(entities)
            .ToList());
    }

    [HttpGet("{id}", Name = "GetOrganizationById")]
    [ProducesResponseType<OrganizationViewModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetOrganizationById(string id)
    {
        var entity = await organizationService.GetById(id);
        return entity == null
            ? NotFound()
            : Ok(mappingService.Map<Organization, OrganizationViewModel>(entity));
    }

    [Authorize(Policy = AuthScopes.Organizations.Write)]
    [HttpPatch("{id}", Name = "UpdateOrganization")]
    [ProducesResponseType<OrganizationViewModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateOrganization(string id, [FromBody] UpdateOrganizationRequest request)
    {
        var entity = await organizationService.Update(id, request);
        return entity == null
            ? NotFound()
            : Ok(mappingService.Map<Organization, OrganizationViewModel>(entity));
    }

    [Authorize(Policy = AuthScopes.Organizations.Write)]
    [HttpDelete("{id}", Name = "DeleteOrganization")]
    [ProducesResponseType<OrganizationViewModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteOrganization(string id)
    {
        var entity = await organizationService.Delete(id);
        return entity == null
            ? NotFound()
            : Ok(mappingService.Map<Organization, OrganizationViewModel>(entity));
    }

    [Authorize(Policy = AuthScopes.Organizations.Write)]
    [HttpPost("", Name = "CreateOrganization")]
    [ProducesResponseType<OrganizationViewModel>(StatusCodes.Status200OK)]
    public async Task<ActionResult> CreateOrganization([FromBody] CreateOrganizationRequest request)
    {
        var entity = await organizationService.Create(request);

        return Ok(mappingService.Map<Organization, OrganizationViewModel>(entity));
    }
}
