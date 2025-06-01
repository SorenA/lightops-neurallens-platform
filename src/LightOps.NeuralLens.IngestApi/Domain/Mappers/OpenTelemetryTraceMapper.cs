using LightOps.Mapping.Api.Mappers;
using LightOps.NeuralLens.Data.Contract.Observability.Models;
using LightOps.NeuralLens.IngestApi.Domain.MappingModels;
using LightOps.NeuralLens.IngestApi.Extensions;
using OpenTelemetry.Proto.Trace.V1;

namespace LightOps.NeuralLens.IngestApi.Domain.Mappers;

public class OpenTelemetryTraceMapper
    : IMapper<ResourceSpans, ObservabilityTraceMappingResult?>
{
    public ObservabilityTraceMappingResult? Map(ResourceSpans source)
    {
        var spans = new List<ObservabilitySpan>();
        var events = new List<ObservabilityEvent>();

        var workspaceId = "-";

        // Map spans
        foreach (var sourceSpan in source.ScopeSpans.SelectMany(x => x.Spans))
        {
            var span = new ObservabilitySpan(
                sourceSpan.SpanId.ToFormattedHexString(),
                sourceSpan.TraceId.ToFormattedHexString(),
                sourceSpan.Name,
                sourceSpan.GetObservabilitySpanKind(),
                sourceSpan.StartTimeUnixNano.ToDateTime(),
                sourceSpan.EndTimeUnixNano.ToDateTime(),
                DateTime.UtcNow,
                DateTime.UtcNow)
            {
                ParentSpanId = sourceSpan.ParentSpanId.ToFormattedHexString(),
            };
            spans.Add(span);

            // Map span events
            foreach (var sourceSpanEvent in sourceSpan.Events)
            {
                var spanEvent = new ObservabilityEvent(
                    Guid.NewGuid().ToString(),
                    span.Id,
                    span.TraceId,
                    sourceSpanEvent.Name,
                    sourceSpanEvent.TimeUnixNano.ToDateTime(),
                    DateTime.UtcNow,
                    DateTime.UtcNow);
                events.Add(spanEvent);
            }
        }

        if (spans.Count == 0)
        {
            // No valid spans
            return null;
        }

        // Map trace
        var trace = new ObservabilityTrace(
            spans.First().TraceId,
            workspaceId,
            source.Resource?.Attributes.GetValueOrDefault("service.name") ?? "Unknown service",
            spans.Min(x => x.StartedAt),
            DateTime.UtcNow,
            DateTime.UtcNow)
        {
            DeploymentEnvironment = source.Resource?.Attributes.GetValueOrDefault(
                "deployment.environment",
                "deployment.environment.name") ?? "default",
            ServiceVersion = source.Resource?.Attributes.GetValueOrDefault("service.version"),
            ServiceInstanceId = source.Resource?.Attributes.GetValueOrDefault("service.instance.id"),
            SessionId = source.Resource?.Attributes.GetValueOrDefault("session.id"),
        };

        return new ObservabilityTraceMappingResult(trace, spans, events);
    }
}