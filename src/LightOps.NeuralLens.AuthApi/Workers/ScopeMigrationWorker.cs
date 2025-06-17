using LightOps.NeuralLens.Component.ServiceDefaults;
using OpenIddict.Abstractions;
using OpenIddict.MongoDb.Models;

namespace LightOps.NeuralLens.AuthApi.Workers;

public class ScopeMigrationWorker(
    ILogger<ScopeMigrationWorker> logger,
    IServiceScopeFactory serviceScopeFactory)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // Update scopes
        await ApplyGlobalScopes();
        await ApplyOrganizationScopes();
        await ApplyPermissionScopes();
        await ApplyWorkspaceScopes();
    }

    private async Task ApplyGlobalScopes()
    {
        await UpsertScope(new OpenIddictScopeDescriptor
        {
            Name = AuthScopes.Profile,
            DisplayName = "Read profile",
        });
    }

    private async Task ApplyOrganizationScopes()
    {
        await UpsertScope(new OpenIddictScopeDescriptor
        {
            Name = AuthScopes.Organizations.Read,
            DisplayName = "Read organizations",
            Resources = { AuthAudiences.OrganizationApi },
        });
        await UpsertScope(new OpenIddictScopeDescriptor
        {
            Name = AuthScopes.Organizations.Write,
            DisplayName = "Write organizations",
            Resources = { AuthAudiences.OrganizationApi },
        });
    }

    private async Task ApplyPermissionScopes()
    {
        await UpsertScope(new OpenIddictScopeDescriptor
        {
            Name = AuthScopes.Permissions.Read,
            DisplayName = "Read permissions",
            Resources = { AuthAudiences.PermissionApi },
        });
        await UpsertScope(new OpenIddictScopeDescriptor
        {
            Name = AuthScopes.Permissions.Write,
            DisplayName = "Write permissions",
            Resources = { AuthAudiences.PermissionApi },
        });
    }

    private async Task ApplyWorkspaceScopes()
    {
        await UpsertScope(new OpenIddictScopeDescriptor
        {
            Name = AuthScopes.Workspaces.Read,
            DisplayName = "Read workspaces",
            Resources = { AuthAudiences.WorkspaceApi },
        });
        await UpsertScope(new OpenIddictScopeDescriptor
        {
            Name = AuthScopes.Workspaces.Write,
            DisplayName = "Write workspaces",
            Resources = { AuthAudiences.WorkspaceApi },
        });
    }

    private async Task UpsertScope(OpenIddictScopeDescriptor scopeDescriptor)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(scopeDescriptor.Name);

        using var scope = serviceScopeFactory.CreateScope();
        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

        // Check if descriptor already exists
        var existingDescriptor = await scopeManager.FindByNameAsync(scopeDescriptor.Name);
        if (existingDescriptor is null)
        {
            // Create descriptor
            await scopeManager.CreateAsync(scopeDescriptor);
            logger.LogInformation($"Created scope '{scopeDescriptor.Name}'.");
            return;
        }

        // Update descriptor
        await scopeManager.UpdateAsync(existingDescriptor, scopeDescriptor);
        logger.LogInformation($"Updated scope '{scopeDescriptor.Name}'.");
    }
}