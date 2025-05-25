namespace LightOps.NeuralLens.WorkspaceApi.Requests;

public record UpdateWorkspaceRequest(
    string Name,
    string? Description);