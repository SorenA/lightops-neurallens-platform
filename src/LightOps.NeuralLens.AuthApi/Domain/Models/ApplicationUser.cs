namespace LightOps.NeuralLens.AuthApi.Domain.Models;

public class ApplicationUser(string id, DateTime createdAt, DateTime updatedAt)
{
    public string Id { get; set; } = id;
    public string? Name { get; set; }
    public string? PictureUrl { get; set; }
    public DateTime CreatedAt { get; set; } = createdAt;
    public DateTime UpdatedAt { get; set; } = updatedAt;

    /// <summary>
    /// Gets or sets the external identifiers for the user, mapped by provider.
    /// </summary>
    public Dictionary<string, string> ExternalIds { get; set; } = new();
}