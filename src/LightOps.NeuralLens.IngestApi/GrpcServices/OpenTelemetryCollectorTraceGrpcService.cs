using Grpc.Core;
using OpenTelemetry.Proto.Collector.Trace.V1;

namespace LightOps.NeuralLens.IngestApi.GrpcServices;

public class OpenTelemetryCollectorTraceGrpcService : TraceService.TraceServiceBase
{
    public override Task<ExportTraceServiceResponse> Export(ExportTraceServiceRequest request, ServerCallContext context)
    {
        // TODO: Implement persistence to somewhere for later processing
        return base.Export(request, context);
    }
}