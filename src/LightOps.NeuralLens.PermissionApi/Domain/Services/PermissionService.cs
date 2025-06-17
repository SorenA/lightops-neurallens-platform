using LightOps.NeuralLens.PermissionApi.Domain.Models;

namespace LightOps.NeuralLens.PermissionApi.Domain.Services;

public class PermissionService(RoleProvider applicationRoleProvider)
{
    /// <summary>
    /// Gets a map of assigned permissions based on user role assignments, keyed by assigned scope.
    /// </summary>
    /// <param name="roleAssignments">The role assignments to build a permission map for.</param>
    /// <returns>A map of assigned permissions, keyed by assigned scope.</returns>
    public Dictionary<string, Permissions?> GetAssignedPermissionsByUserRoleAssignments(params UserRoleAssignment[] roleAssignments)
    {
        var roleIds = roleAssignments
            .Select(assignment => assignment.RoleId)
            .Distinct()
            .ToArray();

        // Get roles
        var roles = applicationRoleProvider.GetRolesByIds(roleIds);

        return roleAssignments
            .ToDictionary(
                k => k.AssignedScope,
                v => roles.FirstOrDefault(r => r.Id == v.RoleId)?.Permissions);
    }
}