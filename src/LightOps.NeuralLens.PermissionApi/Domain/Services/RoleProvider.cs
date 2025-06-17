using LightOps.NeuralLens.Component.ServiceDefaults;
using LightOps.NeuralLens.PermissionApi.Domain.Models;

namespace LightOps.NeuralLens.PermissionApi.Domain.Services;

public class RoleProvider
{
    private readonly List<Role> _roles =
    [
        new("organization-owner", "Organization Owner")
        {
            Permissions = new Permissions
            {
                Actions =
                [
                    AuthPermissionActions.Global.All,
                ],
            },
            AssignableScopes =
            [
                AuthAssignableScopes.Organization,
            ],
        },
        new("organization-contributor", "Organization Contributor")
        {
            Permissions = new Permissions
            {
                Actions =
                [
                    AuthPermissionActions.Global.All,
                ],
                NotActions =
                [
                    AuthPermissionActions.Organizations.Delete,
                    AuthPermissionActions.Workspaces.Delete,
                ],
            },
            AssignableScopes =
            [
                AuthAssignableScopes.Organization,
            ],
        },
        new("organization-reader", "Organization Reader")
        {
            Permissions = new Permissions
            {
                Actions =
                [
                    AuthPermissionActions.Global.Read,
                ],
            },
            AssignableScopes =
            [
                AuthAssignableScopes.Organization,
            ],
        },
    ];

    public List<Role> GetRoles()
    {
        return _roles
            .ToList();
    }

    public Role? GetRolesById(string roleId)
    {
        return GetRolesByIds(roleId)
            .FirstOrDefault();
    }

    public List<Role> GetRolesByIds(params string[] roleIds)
    {
        return _roles
            .Where(x => roleIds.Contains(x.Id))
            .ToList();
    }

    public List<Role> GetRolesByAssignableScope(string assignableScope)
    {
        return _roles
            .Where(x => x.AssignableScopes.Contains(assignableScope))
            .ToList();
    }
}