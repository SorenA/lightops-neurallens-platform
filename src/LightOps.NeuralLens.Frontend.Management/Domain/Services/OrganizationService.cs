using LightOps.NeuralLens.Component.OrganizationApiConnector.Codegen;

namespace LightOps.NeuralLens.Frontend.Management.Domain.Services;

public class OrganizationService(IOrganizationApiClient organizationApiClient)
{
    private readonly Dictionary<string, OrganizationViewModel?> _organizations = new();

    public async Task<OrganizationViewModel?> GetCurrent(string? organizationId)
    {
        if (organizationId == null)
        {
            return null;
        }

        return await organizationApiClient.GetOrganizationByIdAsync(organizationId);

        /*
        // Check if organization is already loaded
        if (_organizations.TryGetValue(organizationId, out var cachedOrganization))
        {
            return cachedOrganization!;
        }

        // Try loading selected organization
        _organizations[organizationId] = await organizationApiClient.GetOrganizationByIdAsync(organizationId);
        if (_organizations.TryGetValue(organizationId, out var fetchedOrganization))
        {
            return fetchedOrganization!;
        }

        // No valid organization selected, select first available
        var availableOrganizations = await organizationApiClient.GetOrganizationsAsync();
        var firstOrganization = availableOrganizations.FirstOrDefault();
        if (firstOrganization != null)
        {
            _organizations[firstOrganization.Id] = firstOrganization;
            return firstOrganization;
        }

        // Create a default organization if none is available
        var defaultOrganization = await organizationApiClient.CreateOrganizationAsync(new CreateOrganizationRequest
        {
            Name = "Default Organization",
            Description = "Default organization created automatically.",
        });

        _organizations[defaultOrganization.Id] = defaultOrganization;
        return defaultOrganization;
        */
    }
}