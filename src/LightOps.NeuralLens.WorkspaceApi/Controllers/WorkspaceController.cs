using LightOps.Mapping.Api.Services;
using LightOps.NeuralLens.WorkspaceApi.Domain.Constants;
using LightOps.NeuralLens.WorkspaceApi.Domain.Exceptions;
using LightOps.NeuralLens.WorkspaceApi.Domain.Models;
using LightOps.NeuralLens.WorkspaceApi.Domain.Services;
using LightOps.NeuralLens.WorkspaceApi.Models;
using LightOps.NeuralLens.WorkspaceApi.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LightOps.NeuralLens.WorkspaceApi.Controllers;

[ApiController]
[Route("workspaces")]
public class WorkspaceController(
    ILogger<WorkspaceController> logger,
    IMappingService mappingService,
    WorkspaceService workspaceService)
    : ControllerBase
{
    [HttpGet("", Name = "GetWorkspaces")]
    [ProducesResponseType<List<WorkspaceViewModel>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetWorkspaces(
        [FromHeader(Name = HeaderNameConstants.OrganizationId)] string organizationId)
    {
        logger.LogInformation("GetWorkspaces called");
        var entities = await workspaceService.GetAll(organizationId);

        return Ok(mappingService.Map<Workspace, WorkspaceViewModel>(entities)
            .ToList());
    }

    [HttpGet("{id}", Name = "GetWorkspaceById")]
    [ProducesResponseType<WorkspaceViewModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetWorkspaceById(
        [FromHeader(Name = HeaderNameConstants.OrganizationId)] string organizationId,
        string id)
    {
        logger.LogInformation("GetWorkspaceById called");
        try
        {
            var entity = await workspaceService.GetById(organizationId, id);
            return Ok(mappingService.Map<Workspace, WorkspaceViewModel>(entity));
        }
        catch (WorkspaceNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPatch("{id}", Name = "UpdateWorkspace")]
    [ProducesResponseType<WorkspaceViewModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateWorkspace(
        [FromHeader(Name = HeaderNameConstants.OrganizationId)] string organizationId,
        string id,
        [FromBody] UpdateWorkspaceRequest request)
    {
        logger.LogInformation("UpdateWorkspace called");
        try
        {
            var entity = await workspaceService.Update(organizationId, id, request);
            return Ok(mappingService.Map<Workspace, WorkspaceViewModel>(entity));
        }
        catch (WorkspaceNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}", Name = "DeleteWorkspace")]
    [ProducesResponseType<WorkspaceViewModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteWorkspace(
        [FromHeader(Name = HeaderNameConstants.OrganizationId)] string organizationId,
        string id)
    {
        logger.LogInformation("DeleteWorkspace called");
        try
        {
            var entity = await workspaceService.Delete(organizationId, id);
            return Ok(mappingService.Map<Workspace, WorkspaceViewModel>(entity));
        }
        catch (WorkspaceNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("", Name = "CreateWorkspace")]
    [ProducesResponseType<WorkspaceViewModel>(StatusCodes.Status200OK)]
    public async Task<ActionResult> CreateWorkspace(
        [FromHeader(Name = HeaderNameConstants.OrganizationId)] string organizationId,
        [FromBody] CreateWorkspaceRequest request)
    {
        logger.LogInformation("CreateWorkspace called");
        var entity = await workspaceService.Create(organizationId, request);

        return Ok(mappingService.Map<Workspace, WorkspaceViewModel>(entity));
    }
}
