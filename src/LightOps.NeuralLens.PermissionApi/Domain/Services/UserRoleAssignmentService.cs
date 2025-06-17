using LightOps.NeuralLens.PermissionApi.Domain.Models;
using LightOps.NeuralLens.PermissionApi.Domain.Repositories;
using LightOps.NeuralLens.PermissionApi.Requests;

namespace LightOps.NeuralLens.PermissionApi.Domain.Services;

public class UserRoleAssignmentService(
    IUserRoleAssignmentRepository userRoleAssignmentRepository,
    AssignedScopeBuilder assignedScopeBuilder)
{
    public async Task<List<UserRoleAssignment>> GetByUserId(string userId)
    {
        return await userRoleAssignmentRepository.GetByUserId(userId);
    }

    public async Task<List<UserRoleAssignment>> GetByUserAssignedScope(string userId, params string[] assignedScopes)
    {
        var upstreamAssignedScopes = assignedScopeBuilder.BuildUpstreamAssignedScopes(assignedScopes);
        return await userRoleAssignmentRepository.GetByUserAssignedScope(userId, upstreamAssignedScopes.ToArray());
    }

    public async Task<List<UserRoleAssignment>> GetByAssignedScope(params string[] assignedScopes)
    {
        var upstreamAssignedScopes = assignedScopeBuilder.BuildUpstreamAssignedScopes(assignedScopes);
        return await userRoleAssignmentRepository.GetByAssignedScope(upstreamAssignedScopes.ToArray());
    }

    public async Task<UserRoleAssignment?> GetById(string id)
    {
        return await userRoleAssignmentRepository.GetById(id);
    }

    public async Task<UserRoleAssignment> Create(CreateUserRoleAssignmentRequest request)
    {
        // Map request to domain model
        var entity = new UserRoleAssignment(
            Guid.NewGuid().ToString(),
            request.UserId,
            request.RoleId,
            request.AssignedScope,
            DateTime.UtcNow);

        return await userRoleAssignmentRepository.Create(entity);
    }

    public async Task<UserRoleAssignment?> Delete(string id)
    {
        var entity = await userRoleAssignmentRepository.GetById(id);
        if (entity == null)
        {
            return null;
        }

        return await userRoleAssignmentRepository.Delete(id);
    }
}