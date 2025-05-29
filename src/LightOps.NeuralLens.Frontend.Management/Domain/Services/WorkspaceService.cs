using LightOps.NeuralLens.Component.WorkspaceApiConnector.Codegen;

namespace LightOps.NeuralLens.Frontend.Management.Domain.Services;
public class WorkspaceService(IWorkspaceApiClient workspaceApiClient)
{
    public async Task<WorkspaceViewModel?> GetCurrent(string? organizationId, string? workspaceId)
    {
        if (organizationId == null || workspaceId == null)
        {
            return null;
        }

        return await workspaceApiClient.GetWorkspaceByIdAsync(organizationId, workspaceId);
    }

    public async Task<WorkspaceViewModel> GetCurrentOrDefault(string? organizationId, string? workspaceId)
    {
        // Try getting selected workspace
        var selectedWorkspace = await GetCurrent(organizationId, workspaceId);
        if (selectedWorkspace != null)
        {
            return selectedWorkspace;
        }

        // No valid workspace selected, select first available
        var availableWorkspaces = await workspaceApiClient.GetWorkspacesAsync(organizationId);
        var firstWorkspace = availableWorkspaces.FirstOrDefault();
        if (firstWorkspace != null)
        {
            return firstWorkspace;
        }

        // Create a default workspace if none is available
        var defaultWorkspace = await workspaceApiClient.CreateWorkspaceAsync(organizationId, new CreateWorkspaceRequest
        {
            Name = "Default Workspace",
            Description = "Default workspace created automatically.",
        });

        return defaultWorkspace;
    }
}