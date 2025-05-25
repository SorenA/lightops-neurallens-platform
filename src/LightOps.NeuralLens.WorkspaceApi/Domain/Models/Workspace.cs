namespace LightOps.NeuralLens.WorkspaceApi.Domain.Models;

public class Workspace(string id, string organizationId, string name, DateTime createdAt, DateTime updatedAt)
{
    public string Id { get; set; } = id;
    public string OrganizationId { get; set; } = organizationId;
    public string Name { get; set; } = name;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = createdAt;
    public DateTime UpdatedAt { get; set; } = updatedAt;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}