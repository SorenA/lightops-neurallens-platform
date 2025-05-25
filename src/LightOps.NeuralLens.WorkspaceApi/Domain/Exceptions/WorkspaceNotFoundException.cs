namespace LightOps.NeuralLens.WorkspaceApi.Domain.Exceptions;

internal class WorkspaceNotFoundException(string workspaceId)
    : Exception($"Workspace with ID {workspaceId} not found.")
{
    public string WorkspaceId { get; } = workspaceId;
}