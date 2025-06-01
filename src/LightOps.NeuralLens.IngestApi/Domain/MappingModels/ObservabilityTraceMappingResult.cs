using LightOps.NeuralLens.Data.Contract.Observability.Models;

namespace LightOps.NeuralLens.IngestApi.Domain.MappingModels;

public class ObservabilityTraceMappingResult(ObservabilityTrace trace, List<ObservabilitySpan> spans, List<ObservabilityEvent> events)
{
    public ObservabilityTrace Trace { get; } = trace;
    public List<ObservabilitySpan> Spans { get; } = spans;
    public List<ObservabilityEvent> Events { get; } = events;
}