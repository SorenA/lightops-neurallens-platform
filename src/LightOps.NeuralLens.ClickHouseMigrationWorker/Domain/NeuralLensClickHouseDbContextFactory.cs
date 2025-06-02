using ClickHouse.Facades;
using ClickHouse.Facades.Migrations;

namespace LightOps.NeuralLens.ClickHouseMigrationWorker.Domain;

public class NeuralLensClickHouseDbContextFactory(
    IClickHouseMigrationInstructions migrationInstructions,
    IHttpClientFactory httpClientFactory)
    : ClickHouseContextFactory<NeuralLensClickHouseDbContext>
{
    protected override void SetupContextOptions(ClickHouseContextOptionsBuilder<NeuralLensClickHouseDbContext> optionsBuilder)
    {
        optionsBuilder
            .WithConnectionString(migrationInstructions.GetConnectionString())
            .WithHttpClientFactory(httpClientFactory, "ClickHouseClient")
            .AllowDatabaseChanges();
    }
}