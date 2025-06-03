using ClickHouse.Facades;
using Google.Api;
using Grpc.Core;
using LightOps.Mapping.Api.Services;
using LightOps.NeuralLens.IngestApi.Domain.DbContexts;
using LightOps.NeuralLens.IngestApi.Domain.MappingModels;
using OpenTelemetry.Proto.Collector.Trace.V1;
using OpenTelemetry.Proto.Trace.V1;

namespace LightOps.NeuralLens.IngestApi.GrpcServices;

public class OpenTelemetryCollectorTraceGrpcService(
    IMappingService mappingService,
    IClickHouseContextFactory<OlapDbContext> clickHouseContextFactory)
    : TraceService.TraceServiceBase
{
    public override async Task<ExportTraceServiceResponse> Export(ExportTraceServiceRequest request, ServerCallContext context)
    {
        await using var olapDbContext = await clickHouseContextFactory.CreateContextAsync();

        // Map OpenTelemetry ResourceSpans to traces, spans and events
        var results = mappingService
            .Map<ResourceSpans, ObservabilityTraceMappingResult>(request.ResourceSpans)
            .ToList();

        // Insert entities into the database
        await olapDbContext.Traces.BulkInsertAsync(results
            .Select(x => x.Trace));
        await olapDbContext.Spans.BulkInsertAsync(results
            .SelectMany(x => x.Spans));
        await olapDbContext.Events.BulkInsertAsync(results
            .SelectMany(x => x.Events));

        return new ExportTraceServiceResponse();
    }
}