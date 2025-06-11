namespace LightOps.NeuralLens.Component.ServiceDefaults;

/// <summary>
/// Defines standard claims from IANA.<br />
/// See https://www.iana.org/assignments/jwt/jwt.xhtml#claims for more information.
/// </summary>
public static class AuthClaims
{
    /// <summary>
    /// Full name.
    /// </summary>
    public const string Name = "name";

    /// <summary>
    /// Profile picture URL.
    /// </summary>
    public const string Picture = "picture";

    /// <summary>
    /// A datetime indicating when the last update was made.
    /// </summary>
    public const string UpdatedAt = "updated_at";
}