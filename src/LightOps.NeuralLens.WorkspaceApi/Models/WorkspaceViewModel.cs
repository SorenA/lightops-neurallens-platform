namespace LightOps.NeuralLens.WorkspaceApi.Models;

public class WorkspaceViewModel(string id, string organizationId, string name, string ingestKey, DateTime createdAt, DateTime updatedAt)
{
    public string Id { get; set; } = id;
    public string OrganizationId { get; set; } = organizationId;
    public string Name { get; set; } = name;
    public string Description { get; set; } = string.Empty;
    public string IngestKey { get; set; } = ingestKey;
    public DateTime CreatedAt { get; set; } = createdAt;
    public DateTime UpdatedAt { get; set; } = updatedAt;
}