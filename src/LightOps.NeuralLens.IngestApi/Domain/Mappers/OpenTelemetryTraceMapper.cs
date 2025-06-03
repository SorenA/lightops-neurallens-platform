using Google.Protobuf.Collections;
using LightOps.Mapping.Api.Mappers;
using LightOps.NeuralLens.Data.Contract.Observability.Models;
using LightOps.NeuralLens.IngestApi.Domain.MappingModels;
using LightOps.NeuralLens.IngestApi.Extensions;
using OpenTelemetry.Proto.Common.V1;
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
            var span = MapSpan(sourceSpan, workspaceId);
            spans.Add(span);

            // Map span events
            events.AddRange(sourceSpan.Events
                .Select(sourceSpanEvent => MapEvent(span, workspaceId, sourceSpanEvent)));
        }

        if (spans.Count == 0)
        {
            // No valid spans
            return null;
        }

        // Map trace
        var trace = MapTrace(source, spans, workspaceId);

        return new ObservabilityTraceMappingResult(trace, spans, events);
    }

    private static ObservabilityTrace MapTrace(ResourceSpans source, List<ObservabilitySpan> spans, string workspaceId)
    {
        return new ObservabilityTrace(
            spans.First().TraceId,
            workspaceId,
            source.Resource?.Attributes.GetValueOrDefault("service.name") ?? "Unknown service",
            spans.Min(x => x.StartedAt),
            DateTime.UtcNow,
            DateTime.UtcNow)
        {
            // TODO: Map input and output
            DeploymentEnvironment = source.Resource?.Attributes.GetValueOrDefault(
                "deployment.environment",
                "deployment.environment.name") ?? "default",
            ServiceVersion = source.Resource?.Attributes.GetValueOrDefault("service.version"),
            ServiceInstanceId = source.Resource?.Attributes.GetValueOrDefault("service.instance.id"),
            SessionId = source.Resource?.Attributes.GetValueOrDefault("session.id"),
        };
    }

    private static ObservabilitySpan MapSpan(Span sourceSpan, string workspaceId)
    {
        var span = new ObservabilitySpan(
            sourceSpan.SpanId.ToFormattedHexString(),
            sourceSpan.TraceId.ToFormattedHexString(),
            workspaceId,
            sourceSpan.Name,
            sourceSpan.GetObservabilitySpanKind(),
            sourceSpan.StartTimeUnixNano.ToDateTime(),
            sourceSpan.EndTimeUnixNano.ToDateTime(),
            DateTime.UtcNow,
            DateTime.UtcNow)
        {
            ParentSpanId = sourceSpan.ParentSpanId.ToFormattedHexString(),
            Type = MapSpanType(sourceSpan),
            Metadata = MapAttributes(sourceSpan.Attributes, "gen_ai.", "url.", "server.", "http.", "network."),
        };

        // TODO: Map input and output
        return span;
    }

    private static ObservabilitySpanType MapSpanType(Span sourceSpan)
    {
        return sourceSpan.Attributes.GetValueOrDefault("gen_ai.operation.name") switch
        {
            null => ObservabilitySpanType.Unspecified,
            "embeddings" => ObservabilitySpanType.GenAiEmbeddings,
            "execute_tool" => ObservabilitySpanType.GenAiExecuteTool,
            "invoke_agent" => ObservabilitySpanType.GenAiInvokeAgent,
            _ => ObservabilitySpanType.GenAiInference,
        };
    }

    private static ObservabilityEvent MapEvent(ObservabilitySpan span, string workspaceId, Span.Types.Event sourceSpanEvent)
    {
        return new ObservabilityEvent(
            Guid.NewGuid().ToString(),
            span.Id,
            span.TraceId,
            workspaceId,
            sourceSpanEvent.Name,
            sourceSpanEvent.TimeUnixNano.ToDateTime(),
            DateTime.UtcNow,
            DateTime.UtcNow)
        {
            // TODO: Map input and output
            Type = MapEventType(sourceSpanEvent),
            Metadata = MapAttributes(sourceSpanEvent.Attributes, "gen_ai."),
        };
    }

    private static ObservabilityEventType MapEventType(Span.Types.Event sourceSpanEvent)
    {
        return sourceSpanEvent.Name switch
        {
            "gen_ai.system.message" => ObservabilityEventType.GenAiSystemMessage,
            "gen_ai.user.message" => ObservabilityEventType.GenAiUserMessage,
            "gen_ai.assistant.message" => ObservabilityEventType.GenAiAssistantMessage,
            "gen_ai.tool.message" => ObservabilityEventType.GenAiToolMessage,
            "gen_ai.choice" => ObservabilityEventType.GenAiChoice,
            _ => ObservabilityEventType.Unspecified,
        };
    }

    private static Dictionary<string, string> MapAttributes(RepeatedField<KeyValue> attributes, params string[] keyPrefixes)
    {
        return attributes
            .Where(x => keyPrefixes
                .Any(kp => x.Key.StartsWith(kp, StringComparison.InvariantCultureIgnoreCase)))
            .Select(x => new { x.Key, Value = attributes.GetValueOrDefault(x.Key) })
            .Where(x => x.Value != null)
            .ToDictionary(x => x.Key, x => x.Value!);
    }
}