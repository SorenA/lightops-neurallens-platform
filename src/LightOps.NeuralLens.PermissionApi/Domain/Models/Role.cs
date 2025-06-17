using LightOps.NeuralLens.Component.ServiceDefaults;

namespace LightOps.NeuralLens.PermissionApi.Domain.Models;

public class Role(string id, string name)
{
    /// <summary>
    /// Gets or sets the unique identifier for the role.
    /// </summary>
    public string Id { get; set; } = id;

    /// <summary>
    /// Gets or sets the name of the role.
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// Gets or sets the permissions of the role.
    /// </summary>
    public Permissions Permissions { get; set; } = new();

    /// <summary>
    /// Gets or sets the assignable scopes of the role. See <see cref="AuthAssignedScopes"/> for predefined scopes.
    /// </summary>
    public List<string> AssignableScopes { get; set; } = [];
}