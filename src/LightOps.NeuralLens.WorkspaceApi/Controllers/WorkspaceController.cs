using Asp.Versioning;
using LightOps.Mapping.Api.Services;
using LightOps.NeuralLens.Component.ServiceDefaults;
using LightOps.NeuralLens.WorkspaceApi.Domain.Constants;
using LightOps.NeuralLens.WorkspaceApi.Domain.Models;
using LightOps.NeuralLens.WorkspaceApi.Domain.Services;
using LightOps.NeuralLens.WorkspaceApi.Models;
using LightOps.NeuralLens.WorkspaceApi.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LightOps.NeuralLens.WorkspaceApi.Controllers;

[ApiVersion(1.0)]
[ApiController]
[Tags("Workspaces")]
[Route("v{version:apiVersion}/workspaces")]
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
        if (ingestKeyService.IsFormatValid(id))
        {
            logger.LogInformation("GetWorkspaceById called with Ingest Key");
            // We assume the ID is an Ingest Key
            var entity = await workspaceService.GetByIngestKey(organizationId, id);
            return entity == null
                ? NotFound()
                : Ok(mappingService.Map<Workspace, WorkspaceViewModel>(entity));
        }
        else
        {
            logger.LogInformation("GetWorkspaceById called");
            // We assume the ID is a workspace ID
            var entity = await workspaceService.GetById(organizationId, id);
            return entity == null
                ? NotFound()
                : Ok(mappingService.Map<Workspace, WorkspaceViewModel>(entity));
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
        var entity = await workspaceService.Update(organizationId, id, request);
        return entity == null
            ? NotFound()
            : Ok(mappingService.Map<Workspace, WorkspaceViewModel>(entity));
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
        var entity = await workspaceService.Delete(organizationId, id);
        return entity == null
            ? NotFound()
            : Ok(mappingService.Map<Workspace, WorkspaceViewModel>(entity));
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
        var entity = await workspaceService.Create(organizationId, request);

        return Ok(mappingService.Map<Workspace, WorkspaceViewModel>(entity));
    }
}
