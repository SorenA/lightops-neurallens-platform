namespace LightOps.NeuralLens.Frontend.Management.Domain.Helpers;

public static class RouteUris
{
    // Root
    public static string Home => "/";

    // Organizations
    public static string Organization(string organizationId) => $"/org/{organizationId}";

    // Workspaces
    public static string Workspace(string organizationId, string workspaceId) =>
        $"{Organization(organizationId)}/space/{workspaceId}";
}