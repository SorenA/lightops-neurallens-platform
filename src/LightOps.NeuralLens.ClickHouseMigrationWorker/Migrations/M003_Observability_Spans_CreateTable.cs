using ClickHouse.Facades.Migrations;
using LightOps.NeuralLens.ClickHouseMigrationWorker.Domain;

namespace LightOps.NeuralLens.ClickHouseMigrationWorker.Migrations;

[ClickHouseMigration(3, nameof(M003_Observability_Spans_CreateTable))]
public class M003_Observability_Spans_CreateTable : ClickHouseMigration
{
    protected override void Up(ClickHouseMigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddRawSqlStatement($@"
            CREATE TABLE {OlapMigrationInstructions.ObservabilityDb}.spans (
                `id` String,
                `trace_id` String,
                `workspace_id` LowCardinality(String),
                `parent_span_id` Nullable(String),
                `name` String,
                `type` Enum(
                    'Unspecified' = 0,
                    'GenAiInference' = 1,
                    'GenAiEmbeddings' = 2,
                    'GenAiExecuteTool' = 3,
                    'GenAiInvokeAgent' = 4
                ),
                `kind` Enum(
                    'Unspecified' = 0,
                    'Internal' = 1,
                    'Server' = 2,
                    'Client' = 3,
                    'Producer' = 4,
                    'Consumer' = 5
                ),
                `started_at` DateTime64(3),
                `ended_at` DateTime64(3),
                `input` Nullable(String) CODEC(ZSTD(3)),
                `output` Nullable(String) CODEC(ZSTD(3)),
                `created_at` DateTime64(3) DEFAULT now(),
                `updated_at` DateTime64(3) DEFAULT now(),
                `tags` Array(String),
                `metadata` Map(LowCardinality(String), String),
                INDEX idx_id id TYPE bloom_filter(0.001) GRANULARITY 1,
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
        migrationBuilder.AddRawSqlStatement($"DROP TABLE IF EXISTS {OlapMigrationInstructions.ObservabilityDb}.spans");
    }
}