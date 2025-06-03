using ClickHouse.Facades.Migrations;
using LightOps.NeuralLens.ClickHouseMigrationWorker.Domain;

namespace LightOps.NeuralLens.ClickHouseMigrationWorker.Migrations;

[ClickHouseMigration(4, nameof(M004_Observability_Events_CreateTable))]
public class M004_Observability_Events_CreateTable : ClickHouseMigration
{
    protected override void Up(ClickHouseMigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddRawSqlStatement($@"
            CREATE TABLE {OlapMigrationInstructions.ObservabilityDb}.events (
                `id` String,
                `span_id` String,
                `trace_id` String,
                `workspace_id` LowCardinality(String),
                `name` String,
                `type` Enum(
                    'Unspecified' = 0,
                    'GenAiSystemMessage' = 1,
                    'GenAiUserMessage' = 2,
                    'GenAiAssistantMessage' = 3,
                    'GenAiToolMessage' = 4,
                    'GenAiChoice' = 5
                ),
                `started_at` DateTime64(3),
                `created_at` DateTime64(3) DEFAULT now(),
                `updated_at` DateTime64(3) DEFAULT now(),
                `tags` Array(String),
                `metadata` Map(LowCardinality(String), String),
                INDEX idx_id id TYPE bloom_filter(0.001) GRANULARITY 1,
                INDEX idx_span_id span_id TYPE bloom_filter() GRANULARITY 1,
                INDEX idx_trace_id trace_id TYPE bloom_filter() GRANULARITY 1,
                INDEX idx_workspace_id workspace_id TYPE bloom_filter() GRANULARITY 1,
                INDEX idx_metadata_key mapKeys(metadata) TYPE bloom_filter(0.01) GRANULARITY 1,
                INDEX idx_metadata_value mapValues(metadata) TYPE bloom_filter(0.01) GRANULARITY 1
            ) ENGINE = ReplacingMergeTree(updated_at)
            PARTITION BY toYYYYMM(started_at)
            PRIMARY KEY (
                 trace_id,
                 toDate(started_at)
            )
            ORDER BY (
                trace_id,
                toDate(started_at),
                id
            )
            ");
    }

    protected override void Down(ClickHouseMigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddRawSqlStatement($"DROP TABLE IF EXISTS {OlapMigrationInstructions.ObservabilityDb}.events");
    }
}