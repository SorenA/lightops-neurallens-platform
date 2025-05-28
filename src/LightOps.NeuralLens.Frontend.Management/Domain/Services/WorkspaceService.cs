using LightOps.NeuralLens.Component.WorkspaceApiConnector.Codegen;

namespace LightOps.NeuralLens.Frontend.Management.Domain.Services;
public class WorkspaceService(IWorkspaceApiClient workspaceApiClient)
{
    private readonly Dictionary<string, Dictionary<string, WorkspaceViewModel?>> _workspacesByOrganizationIds = new();

    public async Task<WorkspaceViewModel?> GetCurrent(string? organizationId, string? workspaceId)
    {
        if (organizationId == null || workspaceId == null)
        {
            return null;
        }

        return await workspaceApiClient.GetWorkspaceByIdAsync(organizationId, workspaceId);

        /*
        if (!_workspacesByOrganizationIds.ContainsKey(organizationId))
        {
            _workspacesByOrganizationIds.Add(organizationId, new Dictionary<string, WorkspaceViewModel?>());
        }

        var workspaces = _workspacesByOrganizationIds[organizationId];

        // Check if workspace is already loaded
        if (workspaces.TryGetValue(workspaceId, out var cachedWorkspace))
        {
            return cachedWorkspace!;
        }

        // Try loading selected workspace
        workspaces[organizationId] = await workspaceApiClient.GetWorkspaceByIdAsync(organizationId, workspaceId);
        if (workspaces.TryGetValue(workspaceId, out var fetchedWorkspace))
        {
            return fetchedWorkspace!;
        }

        // No valid workspace selected, select first available
        var availableWorkspaces = await workspaceApiClient.GetWorkspacesAsync(organizationId);
        var firstWorkspace = availableWorkspaces.FirstOrDefault();
        if (firstWorkspace != null)
        {
            workspaces[firstWorkspace.Id] = firstWorkspace;
            return firstWorkspace;
        }

        // Create a default workspace if none is available
        var defaultWorkspace = await workspaceApiClient.CreateWorkspaceAsync(organizationId, new CreateWorkspaceRequest
        {
            Name = "Default Workspace",
            Description = "Default workspace created automatically.",
        });

        workspaces[defaultWorkspace.Id] = defaultWorkspace;
        return defaultWorkspace;
        */
    }
}