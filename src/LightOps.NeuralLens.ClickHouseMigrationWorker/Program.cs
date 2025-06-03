using ClickHouse.Facades;
using LightOps.NeuralLens.ClickHouseMigrationWorker;
using LightOps.NeuralLens.ClickHouseMigrationWorker.Domain;
using LightOps.NeuralLens.Component.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<MigrationWorker>();
builder.AddServiceDefaults();

// Add ClickHouse migrations
builder.Services.AddClickHouseMigrations<OlapMigrationInstructions, OlapMigrationsLocator>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
        tracing
            .AddSource("LightOps.*")
            .AddSource("ClickHouse.*"));

var host = builder.Build();
host.Run();
