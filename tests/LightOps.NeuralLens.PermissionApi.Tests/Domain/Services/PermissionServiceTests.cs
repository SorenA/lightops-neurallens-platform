using LightOps.NeuralLens.Component.ServiceDefaults;
using LightOps.NeuralLens.PermissionApi.Domain.Models;
using LightOps.NeuralLens.PermissionApi.Domain.Services;

namespace LightOps.NeuralLens.PermissionApi.Tests.Domain.Services;

public class PermissionServiceTests
{
    [Fact]
    public void GetAssignedPermissionsByScope_ReturnsCorrectPermissions_ForValidRoleAssignments()
    {
        // Arrange
        var roleProvider = new RoleProvider();
        var permissionService = new PermissionService(roleProvider);

        var roleAssignments = new[]
        {
            new UserRoleAssignment("ura-1", "userId", "organization-owner", AuthAssignedScopes.Organization("org1"), DateTime.UtcNow),
        };
        // Act
        var permissionsByScope = permissionService.GetAssignedPermissionsByUserRoleAssignments(roleAssignments);
        // Assert
        Assert.NotNull(permissionsByScope);
        Assert.Single(permissionsByScope);

        Assert.Contains(AuthAssignedScopes.Organization("org1"), permissionsByScope.Keys);

        Assert.NotNull(permissionsByScope[AuthAssignedScopes.Organization("org1")]);
    }

    [Fact]
    public void GetAssignedPermissionsByScope_ReturnsEmpty_ForNoRoleAssignments()
    {
        // Arrange
        var roleProvider = new RoleProvider();
        var permissionService = new PermissionService(roleProvider);
        // Act
        var permissionsByScope = permissionService.GetAssignedPermissionsByUserRoleAssignments();

        // Assert
        Assert.NotNull(permissionsByScope);
        Assert.Empty(permissionsByScope);
    }

    [Fact]
    public void GetAssignedPermissionsByScope_ReturnsNull_ForUnknownRoleAssignments()
    {
        // Arrange
        var roleProvider = new RoleProvider();
        var permissionService = new PermissionService(roleProvider);
        var roleAssignments = new[]
        {
            new UserRoleAssignment("ura-1", "userId", "unknown-role", AuthAssignedScopes.Organization("org1"), DateTime.UtcNow),
        };
        // Act
        var permissionsByScope = permissionService.GetAssignedPermissionsByUserRoleAssignments(roleAssignments);
        // Assert
        Assert.NotNull(permissionsByScope);
        Assert.Single(permissionsByScope);
        Assert.Null(permissionsByScope[AuthAssignedScopes.Organization("org1")]);
    }
}