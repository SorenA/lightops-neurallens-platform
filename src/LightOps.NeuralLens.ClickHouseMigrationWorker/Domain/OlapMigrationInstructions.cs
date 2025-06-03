using ClickHouse.Facades.Migrations;

namespace LightOps.NeuralLens.ClickHouseMigrationWorker.Domain;

public class OlapMigrationInstructions(
    IConfiguration configuration)
    : IClickHouseMigrationInstructions
{
    public static string ObservabilityDb => "observability_db";

    private readonly string _connectionString = configuration.GetConnectionString("clickhouse")
                                                ?? throw new ArgumentNullException(
                                                    "clickhouse connection string is not configured.");

    public string GetConnectionString()
    {
        return _connectionString;
    }

    public string DatabaseName => "migrations_db";
    public bool RollbackOnMigrationFail => true;
}