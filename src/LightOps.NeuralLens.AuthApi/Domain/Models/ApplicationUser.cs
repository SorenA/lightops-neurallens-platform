namespace LightOps.NeuralLens.AuthApi.Domain.Models;

public class ApplicationUser(string id, DateTime createdAt, DateTime updatedAt)
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    public string Id { get; set; } = id;

    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the URL of the user's profile picture.
    /// </summary>
    public string? PictureUrl { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the user was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = createdAt;

    /// <summary>
    /// Gets or sets the date and time when the user was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = updatedAt;

    /// <summary>
    /// Gets or sets the external identifiers for the user, mapped by provider.
    /// </summary>
    public Dictionary<string, string> ExternalIds { get; set; } = new();
}