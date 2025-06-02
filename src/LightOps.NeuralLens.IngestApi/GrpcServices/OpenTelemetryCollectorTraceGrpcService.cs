using Grpc.Core;
using LightOps.Mapping.Api.Services;
using LightOps.NeuralLens.IngestApi.Domain.MappingModels;
using OpenTelemetry.Proto.Collector.Trace.V1;
using OpenTelemetry.Proto.Trace.V1;

namespace LightOps.NeuralLens.IngestApi.GrpcServices;

public class OpenTelemetryCollectorTraceGrpcService(
    IMappingService mappingService)
    : TraceService.TraceServiceBase
{
    public override Task<ExportTraceServiceResponse> Export(ExportTraceServiceRequest request, ServerCallContext context)
    {
        var results = mappingService.Map<ResourceSpans, ObservabilityTraceMappingResult>(request.ResourceSpans);

        return Task.FromResult(new ExportTraceServiceResponse());
    }
}