using ClickHouse.Facades;

namespace LightOps.NeuralLens.ClickHouseMigrationWorker.Domain;

public class NeuralLensClickHouseDbContextFactory(
    IHttpClientFactory httpClientFactory)
    : ClickHouseContextFactory<NeuralLensClickHouseDbContext>
{
    protected override void SetupContextOptions(ClickHouseContextOptionsBuilder<NeuralLensClickHouseDbContext> optionsBuilder)
    {
        optionsBuilder
            .WithConnectionString("clickhouse-db")
            .WithHttpClientFactory(httpClientFactory, "ClickHouseClient")
            .AllowDatabaseChanges();
    }
}