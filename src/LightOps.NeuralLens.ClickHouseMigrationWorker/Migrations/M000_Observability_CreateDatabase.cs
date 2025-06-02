using ClickHouse.Facades.Migrations;

namespace LightOps.NeuralLens.ClickHouseMigrationWorker.Migrations;

[ClickHouseMigration(0, nameof(M000_Observability_CreateDatabase))]
public class M000_Observability_CreateDatabase : ClickHouseMigration
{
    protected override void Up(ClickHouseMigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddRawSqlStatement(@"
            CREATE DATABASE IF NOT EXISTS observability_db
            ");
    }

    protected override void Down(ClickHouseMigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddRawSqlStatement("DROP DATABASE IF EXISTS observability_db");
    }
}