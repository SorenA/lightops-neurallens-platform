namespace LightOps.NeuralLens.PermissionApi.Domain.Models;

public class Permissions
{
    /// <summary>
    /// Gets or sets the granted actions.
    /// </summary>
    public List<string> Actions { get; set; } = [];

    /// <summary>
    /// Gets or sets the restricted actions.
    /// </summary>
    public List<string> NotActions { get; set; } = [];
}