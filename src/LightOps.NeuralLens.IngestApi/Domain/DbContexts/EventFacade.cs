using ClickHouse.Facades;
using LightOps.NeuralLens.Data.Contract.Observability.Models;

namespace LightOps.NeuralLens.IngestApi.Domain.DbContexts;

public class EventFacade : ClickHouseFacade<OlapDbContext>
{
    public async Task BulkInsertAsync(
        IEnumerable<ObservabilityEvent> entities,
        CancellationToken cancellationToken = default)
    {
        await BulkInsertAsync(
            $"{OlapDbContext.ObservabilityDb}.events",
            entities.Select(entity => new object[]
            {
                entity.Id,
                entity.SpanId,
                entity.TraceId,
                entity.WorkspaceId,
                entity.Name,
                entity.Type,
                entity.StartedAt,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.Tags,
                entity.Metadata,
            }),
            [
                "id",
                "span_id",
                "trace_id",
                "workspace_id",
                "name",
                "type",
                "started_at",
                "created_at",
                "updated_at",
                "tags",
                "metadata",
            ],
            cancellationToken: cancellationToken);
    }
}