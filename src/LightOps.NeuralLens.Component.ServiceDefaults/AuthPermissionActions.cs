namespace LightOps.NeuralLens.Component.ServiceDefaults;

/// <summary>
/// Defines the actions used in the application for fine-tuned resource permissions.
/// </summary>
public static class AuthPermissionActions
{
    public static class Global
    {
        public const string All = "*";
        public const string Read = "*/read";
    }

    public static class Organizations
    {
        public const string All = "organizations/*";
        public const string Read = "organizations/read";
        public const string Write = "organizations/write";
        public const string Delete = "organizations/delete";
    }

    public static class Workspaces
    {
        public const string All = "workspaces/*";
        public const string Read = "workspaces/read";
        public const string Write = "workspaces/write";
        public const string Delete = "workspaces/delete";
    }
}