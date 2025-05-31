namespace LightOps.NeuralLens.WorkspaceApi.Domain.Exceptions;

internal class WorkspaceNotFoundException(string? message = null)
    : Exception(message ?? $"Workspace not found.")
{
    public string? WorkspaceId { get; internal set; }
    public string? WorkspaceIngestKey { get; internal set; }

    public static WorkspaceNotFoundException FromId(string workspaceId)
    {
        return new WorkspaceNotFoundException($"Workspace with ID {workspaceId} not found.")
        {
            WorkspaceId = workspaceId,
        };
    }

    public static WorkspaceNotFoundException FromIngestKey(string workspaceIngestKey)
    {
        return new WorkspaceNotFoundException($"Workspace with Ingest Key {workspaceIngestKey} not found.")
        {
            WorkspaceIngestKey = workspaceIngestKey,
        };
    }
}