using ClickHouse.Facades;
using LightOps.NeuralLens.Data.Contract.Observability.Models;

namespace LightOps.NeuralLens.IngestApi.Domain.DbContexts;

public class SpanFacade : ClickHouseFacade<OlapDbContext>
{
    public async Task BulkInsertAsync(
        IEnumerable<ObservabilitySpan> entities,
        CancellationToken cancellationToken = default)
    {
        await BulkInsertAsync(
            $"{OlapDbContext.ObservabilityDb}.spans",
            entities.Select(entity => new object[]
            {
                entity.Id,
                entity.TraceId,
                entity.WorkspaceId,
                entity.ParentSpanId!,
                entity.Name,
                entity.Type,
                entity.Kind,
                entity.StartedAt,
                entity.EndedAt,
                entity.Input!,
                entity.Output!,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.Tags,
                entity.Metadata,
            }),
            [
                "id",
                "trace_id",
                "workspace_id",
                "parent_span_id",
                "name",
                "type",
                "kind",
                "started_at",
                "ended_at",
                "input",
                "output",
                "created_at",
                "updated_at",
                "tags",
                "metadata",
            ],
            cancellationToken: cancellationToken);
    }
}