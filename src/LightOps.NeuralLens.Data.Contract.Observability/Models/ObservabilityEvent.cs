namespace LightOps.NeuralLens.Data.Contract.Observability.Models;

public class ObservabilityEvent(
    string id,
    string spanId,
    string traceId,
    string name,
    DateTime startedAt,
    DateTime createdAt,
    DateTime updatedAt)
{
    /// <summary>
    /// Gets the unique identifier for the span.
    /// </summary>
    public string Id { get; } = id;

    /// <summary>
    /// Gets the identifier for the span to which the event belongs.
    /// </summary>
    public string SpanId { get; } = spanId;

    /// <summary>
    /// Gets the identifier for the trace to which the event belongs.
    /// </summary>
    public string TraceId { get; } = traceId;

    /// <summary>
    /// Gets or sets the name of the event.
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// Gets or sets the type of the event.
    /// </summary>
    public ObservabilityEventType Type { get; set; } = ObservabilityEventType.Unspecified;

    /// <summary>
    /// Gets or sets the start time of the event.
    /// </summary>
    public DateTime StartedAt { get; set; } = startedAt;

    /// <summary>
    /// Gets or sets the time when the event was created in NeuralLens.
    /// </summary>
    public DateTime CreatedAt { get; set; } = createdAt;

    /// <summary>
    /// Gets or sets the time when the event was last updated in NeuralLens.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = updatedAt;

    /// <summary>
    /// Gets or sets the list of tags associated with the event.
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Gets or sets the metadata associated with the event.
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();
}
