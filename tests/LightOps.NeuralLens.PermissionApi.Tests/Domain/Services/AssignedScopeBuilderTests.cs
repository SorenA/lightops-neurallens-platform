using LightOps.NeuralLens.Component.ServiceDefaults;
using LightOps.NeuralLens.PermissionApi.Domain.Services;

namespace LightOps.NeuralLens.PermissionApi.Tests.Domain.Services;

public class AssignedScopeBuilderTests
{
    [Fact]
    public void BuildUpstreamAssignedScopes_ShouldReturnMultipleScopes_WhenNoScopesProvided()
    {
        // Arrange
        var builder = new AssignedScopeBuilder();
        // Act
        var result = builder.BuildUpstreamAssignedScopes();
        // Assert
        Assert.Equal(3, result.Count);
        Assert.Contains(AuthAssignableScopes.Global, result);
        Assert.Contains(AuthAssignableScopes.Organization, result);
        Assert.Contains(AuthAssignableScopes.Workspace, result);
    }

    [Fact]
    public void BuildUpstreamAssignedScopes_ShouldReturnMultipleScopes_WhenOneScopeProvided()
    {
        // Arrange
        var builder = new AssignedScopeBuilder();
        // Act
        var result = builder.BuildUpstreamAssignedScopes("scope1");
        // Assert
        Assert.Equal(4, result.Count);
        Assert.Contains(AuthAssignableScopes.Global, result);
        Assert.Contains(AuthAssignableScopes.Organization, result);
        Assert.Contains(AuthAssignableScopes.Workspace, result);
        Assert.Contains("scope1", result);
    }

    [Fact]
    public void BuildUpstreamAssignedScopes_ShouldReturnMultipleScopes_WhenMultipleScopesProvided()
    {
        // Arrange
        var builder = new AssignedScopeBuilder();
        // Act
        var result = builder.BuildUpstreamAssignedScopes("scope1", "scope2");
        // Assert
        Assert.Equal(5, result.Count);
        Assert.Contains(AuthAssignableScopes.Global, result);
        Assert.Contains(AuthAssignableScopes.Organization, result);
        Assert.Contains(AuthAssignableScopes.Workspace, result);
        Assert.Contains("scope1", result);
        Assert.Contains("scope2", result);
    }

    [Fact]
    public void BuildUpstreamAssignedScopes_ShouldReturnMultipleScopes_WhenWorkspaceScopeProvided()
    {
        // Arrange
        var builder = new AssignedScopeBuilder();
        // Act
        var result = builder.BuildUpstreamAssignedScopes("/organizations/org1/workspaces/wsp1");
        // Assert
        Assert.Equal(5, result.Count);
        Assert.Contains(AuthAssignableScopes.Global, result);
        Assert.Contains(AuthAssignableScopes.Organization, result);
        Assert.Contains(AuthAssignableScopes.Workspace, result);
        Assert.Contains("/organizations/org1/workspaces/wsp1", result);
        Assert.Contains("/organizations/org1", result);
    }
}