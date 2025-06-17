namespace LightOps.NeuralLens.Component.ServiceDefaults;

/// <summary>
/// Defines the authorization scopes used in the application for granting general access.
/// </summary>
public static class AuthScopes
{
    public const string Profile = "profile";

    public static class Users
    {
        public const string Read = "users:read";
    }

    public static class Organizations
    {
        public const string Read = "organizations:read";
        public const string Write = "organizations:write";
    }

    public static class Permissions
    {
        public const string Read = "permissions:read";
        public const string Write = "permissions:write";
    }

    public static class Workspaces
    {
        public const string Read = "workspaces:read";
        public const string Write = "workspaces:write";
    }
}