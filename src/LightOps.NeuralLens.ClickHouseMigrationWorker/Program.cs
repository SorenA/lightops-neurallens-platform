using System.Net;
using ClickHouse.Facades;
using LightOps.NeuralLens.ClickHouseMigrationWorker;
using LightOps.NeuralLens.ClickHouseMigrationWorker.Domain;
using LightOps.NeuralLens.Component.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<MigrationWorker>();
builder.AddServiceDefaults();

// Add ClickHouse contexts
builder.Services.AddHttpClient("ClickHouseClient")
    .ConfigureHttpClient((_, httpClient) =>
    {
        httpClient.Timeout = TimeSpan.FromSeconds(60);
    })
    .SetHandlerLifetime(Timeout.InfiniteTimeSpan)
    .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
    {
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
        MaxConnectionsPerServer = 8,
    });
builder.Services.AddClickHouseContext<NeuralLensClickHouseDbContext, NeuralLensClickHouseDbContextFactory>(
    serviceBuilder =>
    {
        serviceBuilder.AddFacade<ObservabilityFacade>();
    });

// Add ClickHouse migrations
builder.Services.AddClickHouseMigrations<NeuralLensClickHouseMigrationInstructions, NeuralLensClickHouseMigrationsLocator>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
        tracing
            .AddSource("LightOps.*")
            .AddSource("ClickHouse.*"));

var host = builder.Build();
host.Run();
