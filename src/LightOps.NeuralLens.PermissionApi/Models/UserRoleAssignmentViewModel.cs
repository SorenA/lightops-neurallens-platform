using LightOps.NeuralLens.Component.ServiceDefaults;

namespace LightOps.NeuralLens.PermissionApi.Models;

public class UserRoleAssignmentViewModel(string id, string userId, string roleId, string assignedScope, DateTime createdAt)
{
    /// <summary>
    /// Gets or sets the unique identifier for the role assignment.
    /// </summary>
    public string Id { get; set; } = id;

    /// <summary>
    /// Gets or sets the ID for the user to whom the role is assigned.
    /// </summary>
    public string UserId { get; set; } = userId;

    /// <summary>
    /// Gets or sets the ID for the role being assigned.
    /// </summary>
    public string RoleId { get; set; } = roleId;

    /// <summary>
    /// Gets or sets the scope to which the role is assigned. See <see cref="AuthAssignedScopes"/> for predefined scopes.
    /// </summary>
    public string AssignedScope { get; set; } = assignedScope;

    /// <summary>
    /// Gets or sets the date and time when the role assignment was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = createdAt;
}