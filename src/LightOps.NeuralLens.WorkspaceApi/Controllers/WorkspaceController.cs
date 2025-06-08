using LightOps.Mapping.Api.Services;
using LightOps.NeuralLens.Component.ServiceDefaults;
using LightOps.NeuralLens.WorkspaceApi.Domain.Constants;
using LightOps.NeuralLens.WorkspaceApi.Domain.Exceptions;
using LightOps.NeuralLens.WorkspaceApi.Domain.Models;
using LightOps.NeuralLens.WorkspaceApi.Domain.Services;
using LightOps.NeuralLens.WorkspaceApi.Models;
using LightOps.NeuralLens.WorkspaceApi.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LightOps.NeuralLens.WorkspaceApi.Controllers;

[ApiController]
[Route("workspaces")]
[Authorize(Policy = AuthScopes.Workspaces.Read)]
public class WorkspaceController(
    ILogger<WorkspaceController> logger,
    IMappingService mappingService,
    WorkspaceService workspaceService,
    IngestKeyService ingestKeyService)
    : ControllerBase
{
    /// <summary>
    /// Get all workspaces for the organization.
    /// </summary>
    /// <param name="organizationId">The ID of the organization.</param>
    /// <returns>A list of workspaces in the organization.</returns>
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

    /// <summary>
    /// Get a workspace by its ID or Ingest Key.
    /// </summary>
    /// <param name="organizationId">The ID of the organization.</param>
    /// <param name="id">The ID of the workspace, or the Ingest Key.</param>
    /// <returns>A workspace in the organization, if found.</returns>
    [HttpGet("{id}", Name = "GetWorkspaceById")]
    [ProducesResponseType<WorkspaceViewModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetWorkspaceById(
        [FromHeader(Name = HeaderNameConstants.OrganizationId)] string organizationId,
        string id)
    {
        try
        {
            if (ingestKeyService.IsFormatValid(id))
            {
                logger.LogInformation("GetWorkspaceById called with Ingest Key");
                // We assume the ID is an Ingest Key
                return Ok(mappingService.Map<Workspace, WorkspaceViewModel>(
                    await workspaceService.GetByIngestKey(organizationId, id)));
            }
            else
            {
                logger.LogInformation("GetWorkspaceById called");
                // We assume the ID is a workspace ID
                return Ok(mappingService.Map<Workspace, WorkspaceViewModel>(
                    await workspaceService.GetById(organizationId, id)));
            }
        }
        catch (WorkspaceNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Update a workspace by its ID.
    /// </summary>
    /// <param name="organizationId">The ID of the organization.</param>
    /// <param name="id">The ID of the workspace, or the Ingest Key.</param>
    /// <param name="request">The update request to apply.</param>
    /// <returns>The updated organization.</returns>
    [Authorize(Policy = AuthScopes.Workspaces.Write)]
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

    /// <summary>
    /// Delete a workspace by its ID.
    /// </summary>
    /// <param name="organizationId">The ID of the organization.</param>
    /// <param name="id">The ID of the workspace, or the Ingest Key.</param>
    /// <returns>The deleted organization.</returns>
    [Authorize(Policy = AuthScopes.Workspaces.Write)]
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

    /// <summary>
    /// Create a new workspace in the organization.
    /// </summary>
    /// <param name="organizationId">The ID of the organization.</param>
    /// <param name="request">The create request to apply.</param>
    /// <returns>The newly created organization.</returns>
    [Authorize(Policy = AuthScopes.Workspaces.Write)]
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
