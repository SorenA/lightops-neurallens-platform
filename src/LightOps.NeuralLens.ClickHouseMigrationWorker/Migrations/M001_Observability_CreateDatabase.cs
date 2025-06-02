using ClickHouse.Facades.Migrations;

namespace LightOps.NeuralLens.ClickHouseMigrationWorker.Migrations;

[ClickHouseMigration(1, nameof(M001_Observability_CreateDatabase))]
public class M001_Observability_CreateDatabase : ClickHouseMigration
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