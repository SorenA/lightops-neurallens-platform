using LightOps.NeuralLens.Component.ServiceDefaults;
using LightOps.NeuralLens.PermissionApi.Domain.Services;

namespace LightOps.NeuralLens.PermissionApi.Tests.Domain.Services;

public class RoleProviderTests
{
    [Fact]
    public void GetRoles_ReturnsAllRoles()
    {
        // Arrange
        var provider = new RoleProvider();
        // Act
        var roles = provider.GetRoles();
        // Assert
        Assert.NotEmpty(roles);
    }

    [Fact]
    public void GetRolesByAssignableScope_ReturnsCorrectRoles_ForOrganizationScope()
    {
        // Arrange
        var provider = new RoleProvider();
        var expectedRoles = new List<string>
        {
            "organization-owner",
            "organization-contributor",
            "organization-reader",
        };

        // Act
        var roles = provider.GetRolesByAssignableScope(AuthAssignableScopes.Organization);

        // Assert
        Assert.Equal(expectedRoles.Count, roles.Count);
        foreach (var expectedRole in expectedRoles)
        {
            Assert.Contains(roles, r => r.Id == expectedRole);
        }
    }

    [Fact]
    public void GetRolesByAssignableScope_ReturnsEmpty_ForUnknownScope()
    {
        // Arrange
        var provider = new RoleProvider();
        // Act
        var roles = provider.GetRolesByAssignableScope("unknown-scope");
        // Assert
        Assert.Empty(roles);
    }

    [Fact]
    public void GetRolesById_ReturnsCorrectRole_ForValidId()
    {
        // Arrange
        var provider = new RoleProvider();
        var roleId = "organization-owner";
        // Act
        var role = provider.GetRolesById(roleId);
        // Assert
        Assert.NotNull(role);
        Assert.Equal("organization-owner", role.Id);
    }

    [Fact]
    public void GetRolesById_ReturnsNull_ForInvalidId()
    {
        // Arrange
        var provider = new RoleProvider();
        var roleId = "invalid-role";
        // Act
        var role = provider.GetRolesById(roleId);
        // Assert
        Assert.Null(role);
    }

    [Fact]
    public void GetRolesByIds_ReturnsCorrectRoles_ForValidIds()
    {
        // Arrange
        var provider = new RoleProvider();
        var roleIds = new[] { "organization-owner", "organization-contributor" };
        // Act
        var roles = provider.GetRolesByIds(roleIds);
        // Assert
        Assert.Equal(2, roles.Count);
        Assert.Contains(roles, r => r.Id == "organization-owner");
        Assert.Contains(roles, r => r.Id == "organization-contributor");
    }

    [Fact]
    public void GetRolesByIds_ReturnsEmpty_ForInvalidIds()
    {
        // Arrange
        var provider = new RoleProvider();
        var roleIds = new[] { "invalid-role" };
        // Act
        var roles = provider.GetRolesByIds(roleIds);
        // Assert
        Assert.Empty(roles);
    }

    [Fact]
    public void GetRolesByIds_ReturnsEmpty_ForNoIds()
    {
        // Arrange
        var provider = new RoleProvider();
        // Act
        var roles = provider.GetRolesByIds();
        // Assert
        Assert.Empty(roles);
    }
}