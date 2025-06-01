using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using Google.Protobuf;
using Google.Protobuf.Collections;
using LightOps.NeuralLens.Data.Contract.Observability.Models;
using OpenTelemetry.Proto.Common.V1;
using OpenTelemetry.Proto.Trace.V1;

namespace LightOps.NeuralLens.IngestApi.Extensions;

public static class OpenTelemetryGrpcModelExtensions
{
    public static string? GetValueOrDefault(this AnyValue? attr)
    {
        if (attr == null)
        {
            return null;
        }

        switch (attr.ValueCase)
        {
            case AnyValue.ValueOneofCase.None:
                return null;
            case AnyValue.ValueOneofCase.StringValue:
                return attr.StringValue;
            case AnyValue.ValueOneofCase.BoolValue:
                return attr.BoolValue.ToString();
            case AnyValue.ValueOneofCase.IntValue:
                return attr.IntValue.ToString();
            case AnyValue.ValueOneofCase.DoubleValue:
                return attr.DoubleValue.ToString(CultureInfo.InvariantCulture);
            case AnyValue.ValueOneofCase.ArrayValue:
                // Map to array of values
                var arrValues = attr.ArrayValue.Values.Select(x => x.GetValueOrDefault());
                return JsonSerializer.Serialize(arrValues);
            case AnyValue.ValueOneofCase.KvlistValue:
                // Map to dictionary with values
                var kvValues = attr.KvlistValue.Values.ToDictionary(x => x.Key, x => x.Value.GetValueOrDefault());
                return JsonSerializer.Serialize(kvValues);
            case AnyValue.ValueOneofCase.BytesValue:
                return "Unsupported attribute type: BytesValue";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static string? GetValueOrDefault(this RepeatedField<KeyValue> attrs, params string[] keys)
    {
        // Find the first attribute that matches any of the provided keys
        var attribute = attrs.FirstOrDefault(attr => keys.Contains(attr.Key, StringComparer.OrdinalIgnoreCase));
        return attribute?.Value.GetValueOrDefault();
    }

    public static ObservabilitySpanKind GetObservabilitySpanKind(this Span span)
    {
        return span.Kind switch
        {
            Span.Types.SpanKind.Unspecified => ObservabilitySpanKind.Unspecified,
            Span.Types.SpanKind.Internal => ObservabilitySpanKind.Internal,
            Span.Types.SpanKind.Server => ObservabilitySpanKind.Server,
            Span.Types.SpanKind.Client => ObservabilitySpanKind.Client,
            Span.Types.SpanKind.Producer => ObservabilitySpanKind.Producer,
            Span.Types.SpanKind.Consumer => ObservabilitySpanKind.Consumer,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static DateTime ToDateTime(this ulong timestampNano)
    {
        var convertedTimestampNano = (long)(timestampNano - 9223372036854775808);
        var timeSpan = TimeSpan.FromTicks(convertedTimestampNano / 100);
        return DateTimeOffset.UnixEpoch.Add(timeSpan).ToUniversalTime().DateTime;
    }

    public static string ToFormattedHexString(this ByteString me, int groupSize = 4)
    {
        var hex = Convert.ToHexString(me.Span).ToLowerInvariant();

        // Group characters
        var groups = Enumerable.Range(0, hex.Length / groupSize)
            .Select(i => hex.Substring(i * groupSize, groupSize));

        return string.Join("-", groups);
    }
}