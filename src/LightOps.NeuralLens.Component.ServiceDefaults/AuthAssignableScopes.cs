namespace LightOps.NeuralLens.Component.ServiceDefaults;

/// <summary>
/// Defines the assignable scopes used in the for RBAC.
/// </summary>
public static class AuthAssignableScopes
{
    public static string Global => AuthAssignedScopes.Global();
    public static string Organization => AuthAssignedScopes.Organization("*");
    public static string Workspace => AuthAssignedScopes.Workspace("*", "*");
}