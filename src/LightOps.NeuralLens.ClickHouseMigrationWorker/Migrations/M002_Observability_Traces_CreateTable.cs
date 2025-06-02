using System.Diagnostics;
using ClickHouse.Facades.Migrations;

namespace LightOps.NeuralLens.ClickHouseMigrationWorker.Migrations;

[ClickHouseMigration(2, nameof(M002_Observability_Traces_CreateTable))]
public class M002_Observability_Traces_CreateTable : ClickHouseMigration
{
    protected override void Up(ClickHouseMigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddRawSqlStatement(@"
            CREATE TABLE observability_db.traces (
                `id` String,
                `workspace_id` LowCardinality(String),
                `service_name` String,
                `service_version` Nullable(String),
                `service_instance_id` Nullable(String),
                `deployment_environment` LowCardinality(String) DEFAULT 'default',
                `session_id` Nullable(String),
                `started_at` DateTime64(3),
                `input` Nullable(String) CODEC(ZSTD(3)),
                `output` Nullable(String) CODEC(ZSTD(3)),
                `created_at` DateTime64(3) DEFAULT now(),
                `updated_at` DateTime64(3) DEFAULT now(),
                `is_deleted` UInt8,
                `deleted_at` Nullable(DateTime64(3)),
                `tags` Array(String),
                `metadata` Map(LowCardinality(String), String),
                INDEX idx_id id TYPE bloom_filter(0.001) GRANULARITY 1,
                INDEX idx_res_metadata_key mapKeys(metadata) TYPE bloom_filter(0.01) GRANULARITY 1,
                INDEX idx_res_metadata_value mapValues(metadata) TYPE bloom_filter(0.01) GRANULARITY 1
            ) ENGINE = ReplacingMergeTree(updated_at)
            PARTITION BY toYYYYMM(started_at)
            PRIMARY KEY (
                 workspace_id,
                 toDate(started_at)
            )
            ORDER BY (
                workspace_id,
                toDate(started_at),
                id
            )
            ");
    }

    protected override void Down(ClickHouseMigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddRawSqlStatement("DROP TABLE IF EXISTS observability_db.traces");
    }
}