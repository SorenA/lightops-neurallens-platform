namespace LightOps.NeuralLens.Data.Contract.Observability.Models;

/// <summary>
/// Represents a trace in the observability system.
/// </summary>
public class ObservabilityTrace(
    string id,
    string workspaceId,
    string serviceName,
    DateTime startedAt,
    DateTime createdAt,
    DateTime updatedAt)
{
    /// <summary>
    /// Gets the unique identifier for the trace.
    /// </summary>
    public string Id { get; } = id;

    /// <summary>
    /// Gets the identifier for the workspace to which the trace belongs.
    /// </summary>
    public string WorkspaceId { get; } = workspaceId;

    /// <summary>
    /// Gets or sets the name of the service the trace originated from.
    /// </summary>
    public string ServiceName { get; set; } = serviceName;

    /// <summary>
    /// Gets or sets the version of the service the trace originated from.
    /// </summary>
    public string? ServiceVersion { get; set; }

    /// <summary>
    /// Gets or sets the instance id of the service the trace originated form.
    /// </summary>
    public string? ServiceInstanceId { get; set; }

    /// <summary>
    /// Gets or sets the deployment environment the trace originated from.
    /// </summary>
    public string DeploymentEnvironment { get; set; } = "default";

    /// <summary>
    /// Gets or sets the identifier for the session associated with the trace, if any.
    /// </summary>
    public string? SessionId { get; set; }

    /// <summary>
    /// Gets or sets the start time of the trace.
    /// </summary>
    public DateTime StartedAt { get; set; } = startedAt;

    /// <summary>
    /// Gets or sets the input data associated with the trace, if any.
    /// </summary>
    public string? Input { get; set; }

    /// <summary>
    /// Gets or sets the output data associated with the trace, if any.
    /// </summary>
    public string? Output { get; set; }

    /// <summary>
    /// Gets or sets the time when the trace was created in NeuralLens.
    /// </summary>
    public DateTime CreatedAt { get; set; } = createdAt;

    /// <summary>
    /// Gets or sets the time when the trace was last updated in NeuralLens.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = updatedAt;

    /// <summary>
    /// Gets or sets a value indicating whether the trace has been deleted.
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Gets or sets the time when the trace was deleted, if applicable.
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Gets or sets the list of tags associated with the trace.
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Gets or sets the metadata associated with the trace.
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();
}