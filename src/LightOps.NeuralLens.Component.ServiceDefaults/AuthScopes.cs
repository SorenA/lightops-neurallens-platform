namespace LightOps.NeuralLens.Component.ServiceDefaults;

public static class AuthScopes
{
    public const string Profile = "profile";

    public static class Organizations
    {
        public const string Read = "organizations:read";
        public const string Write = "organizations:write";
    }

    public static class Workspaces
    {
        public const string Read = "workspaces:read";
        public const string Write = "workspaces:write";
    }
}