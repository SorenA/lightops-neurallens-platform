namespace LightOps.NeuralLens.Data.Contract.Observability.Models;

/// <summary>
/// Represents a span in the observability system.
/// </summary>
public class ObservabilitySpan(
    string id,
    string traceId,
    string workspaceId,
    string name,
    ObservabilitySpanKind kind,
    DateTime startedAt,
    DateTime endedAt,
    DateTime createdAt,
    DateTime updatedAt)
{
    /// <summary>
    /// Gets the unique identifier for the span.
    /// </summary>
    public string Id { get; } = id;

    /// <summary>
    /// Gets the identifier for the trace to which the span belongs.
    /// </summary>
    public string TraceId { get; } = traceId;

    /// <summary>
    /// Gets the identifier for the workspace to which the span belongs.
    /// </summary>
    public string WorkspaceId { get; } = workspaceId;

    /// <summary>
    /// Gets or sets the identifier of the parent span, if any.
    /// </summary>
    public string? ParentSpanId { get; set; }

    /// <summary>
    /// Gets or sets the name of the span.
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// Gets or sets the type of the span.
    /// </summary>
    public ObservabilitySpanType Type { get; set; } = ObservabilitySpanType.Unspecified;

    /// <summary>
    /// Gets or sets the kind of the span.
    /// </summary>
    public ObservabilitySpanKind Kind { get; set; } = kind;

    /// <summary>
    /// Gets or sets the start time of the span.
    /// </summary>
    public DateTime StartedAt { get; set; } = startedAt;

    /// <summary>
    /// Gets or sets the end time of the span.
    /// </summary>
    public DateTime EndedAt { get; set; } = endedAt;

    /// <summary>
    /// Gets the duration of the span.
    /// </summary>
    public TimeSpan Duration => EndedAt - StartedAt;

    public string? Input { get; set; }
    public string? Output { get; set; }

    /// <summary>
    /// Gets or sets the time when the span was created in NeuralLens.
    /// </summary>
    public DateTime CreatedAt { get; set; } = createdAt;

    /// <summary>
    /// Gets or sets the time when the span was last updated in NeuralLens.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = updatedAt;

    /// <summary>
    /// Gets or sets the list of tags associated with the span.
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Gets or sets the metadata associated with the span.
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();
}