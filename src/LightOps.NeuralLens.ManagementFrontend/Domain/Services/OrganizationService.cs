using LightOps.NeuralLens.Component.OrganizationApiConnector.Codegen;

namespace LightOps.NeuralLens.ManagementFrontend.Domain.Services;

public class OrganizationService(IOrganizationApiClient organizationApiClient)
{
    public async Task<OrganizationViewModel?> GetCurrent(string? organizationId)
    {
        if (organizationId == null)
        {
            return null;
        }

        return await organizationApiClient.GetOrganizationByIdAsync(organizationId);
    }

    public async Task<OrganizationViewModel> GetCurrentOrDefault(string? organizationId)
    {
        // Try getting selected organization
        var selectedOrganization = await GetCurrent(organizationId);
        if (selectedOrganization != null)
        {
            return selectedOrganization;
        }

        // No valid organization selected, select first available
        var availableOrganizations = await organizationApiClient.GetOrganizationsAsync();
        var firstOrganization = availableOrganizations.FirstOrDefault();
        if (firstOrganization != null)
        {
            return firstOrganization;
        }

        // Create a default organization if none is available
        var defaultOrganization = await organizationApiClient.CreateOrganizationAsync(new CreateOrganizationRequest
        {
            Name = "Default Organization",
            Description = "Default organization created automatically.",
        });

        return defaultOrganization;
    }
}