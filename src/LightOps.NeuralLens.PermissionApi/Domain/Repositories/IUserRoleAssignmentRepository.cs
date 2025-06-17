using LightOps.NeuralLens.PermissionApi.Domain.Models;

namespace LightOps.NeuralLens.PermissionApi.Domain.Repositories;

public interface IUserRoleAssignmentRepository
{
    Task<List<UserRoleAssignment>> GetByUserId(string userId);
    Task<List<UserRoleAssignment>> GetByUserAssignedScope(string userId, params string[] assignedScopes);
    Task<List<UserRoleAssignment>> GetByAssignedScope(params string[] assignedScopes);
    Task<UserRoleAssignment?> GetById(string id);
    Task<UserRoleAssignment> Create(UserRoleAssignment userRoleAssignment);
    Task<UserRoleAssignment?> Delete(string id);
}