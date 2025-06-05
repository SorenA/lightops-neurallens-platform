using LightOps.NeuralLens.Component.ServiceDefaults;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace LightOps.NeuralLens.AuthApi;

public class SeedWorker(
    ILogger<SeedWorker> logger,
    IServiceScopeFactory serviceScopeFactory,
    IConfiguration configuration)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await SeedApplications();

        // Seed scopes
        await SeedWorkspaceScopes();
        await SeedOrganizationScopes();
    }

    private async Task SeedApplications()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var applicationManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        // Ensure Ingest API client exists
        var ingestApiClientId = configuration.GetValue<string>("Auth:Clients:IngestApi:ClientId")!;
        if (await applicationManager.FindByClientIdAsync(ingestApiClientId) is null)
        {
            await applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = ingestApiClientId,
                ClientSecret = configuration.GetValue<string>("Auth:Clients:IngestApi:ClientSecret")!,
                DisplayName = "Ingest API",
                ClientType = ClientTypes.Confidential,
                Permissions =
                {
                    Permissions.GrantTypes.ClientCredentials,
                    Permissions.ResponseTypes.Token,
                    Permissions.Endpoints.Token,
                    Permissions.Endpoints.Introspection,
                    Permissions.Prefixes.Scope + AuthScopes.Workspaces.Read,
                },
            });

            logger.LogInformation("Created Ingest API client.");
        }

        // Ensure Organization API client exists
        var organizationApiClientId = configuration.GetValue<string>("Auth:Clients:OrganizationApi:ClientId")!;
        if (await applicationManager.FindByClientIdAsync(organizationApiClientId) is null)
        {
            await applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = organizationApiClientId,
                ClientSecret = configuration.GetValue<string>("Auth:Clients:OrganizationApi:ClientSecret")!,
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

            logger.LogInformation("Created Organization API client.");
        }

        // Ensure Workspace API client exists
        var workspaceApiClientId = configuration.GetValue<string>("Auth:Clients:WorkspaceApi:ClientId")!;
        if (await applicationManager.FindByClientIdAsync(workspaceApiClientId) is null)
        {
            await applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = workspaceApiClientId,
                ClientSecret = configuration.GetValue<string>("Auth:Clients:WorkspaceApi:ClientSecret")!,
                DisplayName = "Workspace API",
                ClientType = ClientTypes.Confidential,
                Permissions =
                {
                    Permissions.GrantTypes.ClientCredentials,
                    Permissions.ResponseTypes.Token,
                    Permissions.Endpoints.Token,
                    Permissions.Endpoints.Introspection,
                    Permissions.Prefixes.Scope + AuthScopes.Organizations.Read,
                },
            });

            logger.LogInformation("Created Workspace API client.");
        }
    }

    public async Task SeedOrganizationScopes()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

        if (await scopeManager.FindByNameAsync(AuthScopes.Organizations.Read) is null)
        {
            await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = AuthScopes.Organizations.Read,
                DisplayName = "Read organizations",
                Resources = { AuthAudiences.OrganizationApi },
            });
            logger.LogInformation($"Created scope '{AuthScopes.Organizations.Read}'.");
        }

        if (await scopeManager.FindByNameAsync(AuthScopes.Organizations.Write) is null)
        {
            await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = AuthScopes.Organizations.Write,
                DisplayName = "Write organizations",
                Resources = { AuthAudiences.OrganizationApi },
            });
            logger.LogInformation($"Created scope '{AuthScopes.Organizations.Write}'.");
        }
    }

    public async Task SeedWorkspaceScopes()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

        if (await scopeManager.FindByNameAsync(AuthScopes.Workspaces.Read) is null)
        {
            await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = AuthScopes.Workspaces.Read,
                DisplayName = "Read workspaces",
                Resources = { AuthAudiences.WorkspaceApi },
            });
            logger.LogInformation($"Created scope '{AuthScopes.Workspaces.Read}'.");
        }

        if (await scopeManager.FindByNameAsync(AuthScopes.Workspaces.Write) is null)
        {
            await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = AuthScopes.Workspaces.Write,
                DisplayName = "Write workspaces",
                Resources = { AuthAudiences.WorkspaceApi },
            });
            logger.LogInformation($"Created scope '{AuthScopes.Workspaces.Write}'.");
        }
    }
}