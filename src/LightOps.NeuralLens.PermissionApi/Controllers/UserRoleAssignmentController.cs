using Asp.Versioning;
using LightOps.Mapping.Api.Services;
using LightOps.NeuralLens.Component.ServiceDefaults;
using LightOps.NeuralLens.PermissionApi.Domain.Models;
using LightOps.NeuralLens.PermissionApi.Domain.Services;
using LightOps.NeuralLens.PermissionApi.Models;
using LightOps.NeuralLens.PermissionApi.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LightOps.NeuralLens.PermissionApi.Controllers;

[ApiVersion(1.0)]
[ApiController]
[Tags("Permissions")]
[Route("v{version:apiVersion}/permissions/user-role-assignments")]
[Authorize(Policy = AuthScopes.Permissions.Read)]
public class UserRoleAssignmentController(
    IMappingService mappingService,
    UserRoleAssignmentService userRoleAssignmentService)
    : ControllerBase
{
    [Authorize(Policy = AuthScopes.Permissions.Read)]
    [HttpGet("", Name = "GetByAssignedScope")]
    [ProducesResponseType<List<UserRoleAssignmentViewModel>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetByAssignedScope()
    {
        var entities = await userRoleAssignmentService.GetByAssignedScope();

        return Ok(mappingService.Map<UserRoleAssignment, UserRoleAssignmentViewModel>(entities)
            .ToList());
    }

    [Authorize(Policy = AuthScopes.Permissions.Read)]
    [HttpGet("{id}", Name = "GetUserRoleAssignmentById")]
    [ProducesResponseType<UserRoleAssignmentViewModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetUserRoleAssignmentById(string id)
    {
        var entity = await userRoleAssignmentService.GetById(id);
        return entity == null
            ? NotFound()
            : Ok(mappingService.Map<UserRoleAssignment, UserRoleAssignmentViewModel>(entity));
    }

    [Authorize(Policy = AuthScopes.Permissions.Write)]
    [HttpDelete("{id}", Name = "DeleteUserRoleAssignment")]
    [ProducesResponseType<UserRoleAssignmentViewModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteUserRoleAssignment(string id)
    {
        var entity = await userRoleAssignmentService.Delete(id);
        return entity == null
            ? NotFound()
            : Ok(mappingService.Map<UserRoleAssignment, UserRoleAssignmentViewModel>(entity));
    }

    [Authorize(Policy = AuthScopes.Permissions.Write)]
    [HttpPost("", Name = "CreateUserRoleAssignment")]
    [ProducesResponseType<UserRoleAssignmentViewModel>(StatusCodes.Status200OK)]
    public async Task<ActionResult> CreateUserRoleAssignment([FromBody] CreateUserRoleAssignmentRequest request)
    {
        var entity = await userRoleAssignmentService.Create(request);

        return Ok(mappingService.Map<UserRoleAssignment, UserRoleAssignmentViewModel>(entity));
    }
}
