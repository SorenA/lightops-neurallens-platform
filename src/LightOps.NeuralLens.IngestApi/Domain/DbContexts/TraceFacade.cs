using ClickHouse.Facades;
using LightOps.NeuralLens.Data.Contract.Observability.Models;

namespace LightOps.NeuralLens.IngestApi.Domain.DbContexts;

public class TraceFacade : ClickHouseFacade<OlapDbContext>
{
    public async Task BulkInsertAsync(
        IEnumerable<ObservabilityTrace> entities,
        CancellationToken cancellationToken = default)
    {
        await BulkInsertAsync(
            $"{OlapDbContext.ObservabilityDb}.traces",
            entities.Select(entity => new object[]
            {
                entity.Id,
                entity.WorkspaceId,
                entity.ServiceName,
                entity.ServiceVersion!,
                entity.ServiceInstanceId!,
                entity.DeploymentEnvironment,
                entity.SessionId!,
                entity.StartedAt,
                entity.Input!,
                entity.Output!,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.Tags,
                entity.Metadata,
            }),
            [
                "id",
                "workspace_id",
                "service_name",
                "service_version",
                "service_instance_id",
                "deployment_environment",
                "session_id",
                "started_at",
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