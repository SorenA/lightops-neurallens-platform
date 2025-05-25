namespace LightOps.NeuralLens.OrganizationApi.Domain.Models;

public class Organization(string id, string name, DateTime createdAt, DateTime updatedAt)
{
    public string Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = createdAt;
    public DateTime UpdatedAt { get; set; } = updatedAt;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}