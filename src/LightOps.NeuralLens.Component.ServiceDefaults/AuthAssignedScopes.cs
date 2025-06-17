namespace LightOps.NeuralLens.Component.ServiceDefaults;

/// <summary>
/// Defines the assigned scopes used in the application for fine-tuned resource permissions.
/// </summary>
public static class AuthAssignedScopes
{
    public static string Global() => $"*";

    public static string Organization(string organizationId) =>
        $"/organizations/{organizationId}";

    public static string Workspace(string organizationId, string workspaceId) =>
        $"{Organization(organizationId)}/workspaces/{workspaceId}";
}