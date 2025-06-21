using LightOps.NeuralLens.Component.ServiceDefaults;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace LightOps.NeuralLens.AuthApi.Workers;

public class ApplicationMigrationWorker(
    ILogger<ApplicationMigrationWorker> logger,
    IServiceScopeFactory serviceScopeFactory,
    IConfiguration configuration)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // Update applications
        await ApplyApiApplications();
        await ApplyFrontendApplications();
    }

    private async Task ApplyApiApplications()
    {
        await UpsertApplication(new OpenIddictApplicationDescriptor
        {
            ClientId = configuration.GetValue<string>("Services:ingest-api:ClientId")!,
            ClientSecret = configuration.GetValue<string>("Services:ingest-api:ClientSecret")!,
            DisplayName = "Ingest API",
            ClientType = ClientTypes.Confidential,
            Permissions =
            {
                Permissions.GrantTypes.ClientCredentials,
                Permissions.ResponseTypes.Token,
                Permissions.Endpoints.Token,
                Permissions.Endpoints.Introspection,
            },
        });

        await UpsertApplication(new OpenIddictApplicationDescriptor
        {
            ClientId = configuration.GetValue<string>("Services:organization-api:ClientId")!,
            ClientSecret = configuration.GetValue<string>("Services:organization-api:ClientSecret")!,
            DisplayName = "Organization API",
            ClientType = ClientTypes.Confidential,
            Permissions =
            {
                Permissions.GrantTypes.ClientCredentials,
                Permissions.ResponseTypes.Token,
                Permissions.Endpoints.Token,
                Permissions.Endpoints.Introspection,
            },
        });

        await UpsertApplication(new OpenIddictApplicationDescriptor
        {
            ClientId = configuration.GetValue<string>("Services:permission-api:ClientId")!,
            ClientSecret = configuration.GetValue<string>("Services:permission-api:ClientSecret")!,
            DisplayName = "Permission API",
            ClientType = ClientTypes.Confidential,
            Permissions =
            {
                Permissions.GrantTypes.ClientCredentials,
                Permissions.ResponseTypes.Token,
                Permissions.Endpoints.Token,
                Permissions.Endpoints.Introspection,
            },
        });

        await UpsertApplication(new OpenIddictApplicationDescriptor
        {
            ClientId = configuration.GetValue<string>("Services:workspace-api:ClientId")!,
            ClientSecret = configuration.GetValue<string>("Services:workspace-api:ClientSecret")!,
            DisplayName = "Workspace API",
            ClientType = ClientTypes.Confidential,
            Permissions =
            {
                Permissions.GrantTypes.ClientCredentials,
                Permissions.ResponseTypes.Token,
                Permissions.Endpoints.Token,
                Permissions.Endpoints.Introspection,
            },
        });
    }

    private async Task ApplyFrontendApplications()
    {
        var managementFrontendUrl = configuration.GetValue<string>("Services:management-frontend:Https:0")!;
        await UpsertApplication(new OpenIddictApplicationDescriptor
        {
            ClientId = configuration.GetValue<string>("Services:management-frontend:ClientId")!,
            DisplayName = "Management frontend",
            ClientType = ClientTypes.Public,
            RedirectUris =
            {
                new Uri($"{managementFrontendUrl}/api/auth/sign-in-oidc-callback"),
            },
            PostLogoutRedirectUris =
            {
                new Uri($"{managementFrontendUrl}"),
            },
            Permissions =
            {
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.RefreshToken,
                Permissions.ResponseTypes.Code,
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.Endpoints.EndSession,

                // Scopes
                Permissions.Scopes.Profile,
                Permissions.Prefixes.Scope + AuthScopes.Organizations.Read,
                Permissions.Prefixes.Scope + AuthScopes.Organizations.Write,
                Permissions.Prefixes.Scope + AuthScopes.Workspaces.Read,
                Permissions.Prefixes.Scope + AuthScopes.Workspaces.Write,
            },
            Requirements =
            {
                Requirements.Features.ProofKeyForCodeExchange,
            },
        });

        var documentationFrontendUrl = configuration.GetValue<string>("Services:documentation-frontend:Https:0")!;
        await UpsertApplication(new OpenIddictApplicationDescriptor
        {
            ClientId = configuration.GetValue<string>("Services:documentation-frontend:ClientId")!,
            DisplayName = "Documentation frontend",
            ClientType = ClientTypes.Public,
            RedirectUris =
            {
                new Uri($"{documentationFrontendUrl}/api/"),
            },
            Permissions =
            {
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.RefreshToken,
                Permissions.ResponseTypes.Code,
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,

                // Scopes
                Permissions.Scopes.Profile,
                Permissions.Prefixes.Scope + AuthScopes.Organizations.Read,
                Permissions.Prefixes.Scope + AuthScopes.Organizations.Write,
                Permissions.Prefixes.Scope + AuthScopes.Permissions.Read,
                Permissions.Prefixes.Scope + AuthScopes.Permissions.Write,
                Permissions.Prefixes.Scope + AuthScopes.Workspaces.Read,
                Permissions.Prefixes.Scope + AuthScopes.Workspaces.Write,
            },
            Requirements =
            {
                Requirements.Features.ProofKeyForCodeExchange,
            },
        });
    }

    private async Task UpsertApplication(OpenIddictApplicationDescriptor applicationDescriptor)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(applicationDescriptor.ClientId);

        using var scope = serviceScopeFactory.CreateScope();
        var applicationManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        // Check if descriptor already exists
        var existingDescriptor = await applicationManager.FindByClientIdAsync(applicationDescriptor.ClientId);
        if (existingDescriptor is null)
        {
            // Create descriptor
            await applicationManager.CreateAsync(applicationDescriptor);
            logger.LogInformation($"Created application '{applicationDescriptor.ClientId}'.");
            return;
        }

        // Update descriptor
        await applicationManager.UpdateAsync(existingDescriptor, applicationDescriptor);
        logger.LogInformation($"Updated application '{applicationDescriptor.ClientId}'.");
    }
}