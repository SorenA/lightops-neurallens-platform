namespace LightOps.NeuralLens.WorkspaceApi.Requests;

public record CreateWorkspaceRequest(
    string Name,
    string? Description);