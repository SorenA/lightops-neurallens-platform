namespace LightOps.NeuralLens.OrganizationApi.Domain.Models;

public class Organization(string id, string name, DateTime createdAt, DateTime updatedAt)
{
    /// <summary>
    /// Gets or sets the unique identifier for the organization.
    /// </summary>
    public string Id { get; set; } = id;

    /// <summary>
    /// Gets or sets the name of the organization.
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// Gets or sets the description of the organization.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the organization was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = createdAt;

    /// <summary>
    /// Gets or sets the date and time when the organization was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = updatedAt;

    /// <summary>
    /// Gets or sets a value indicating whether the organization is deleted.
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Gets or sets the date and time when the organization was deleted, if applicable.
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Gets or sets the external identifiers for the user, mapped by provider.
    /// </summary>
    public Dictionary<string, string> UserRoles { get; set; } = new();
}

public enum OrganizationRole
{
    /// <summary>
    /// Indicates that the role is unspecified.
    /// </summary>
    Unspecified = 0,

    /// <summary>
    /// Indicates that the user is the owner of the organization, with destructive permissions.
    /// </summary>
    Owner = 1,

    /// <summary>
    /// Indicates that the user is an administrator of the organization.
    /// </summary>
    Administrator = 2,

    /// <summary>
    /// Indicates that the user is a member of the organization.
    /// </summary>
    Member = 2,
}