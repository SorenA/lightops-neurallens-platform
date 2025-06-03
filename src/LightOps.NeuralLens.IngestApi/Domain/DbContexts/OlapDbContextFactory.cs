using ClickHouse.Facades;

namespace LightOps.NeuralLens.IngestApi.Domain.DbContexts;

public class OlapDbContextFactory(
    IConfiguration configuration,
    IHttpClientFactory httpClientFactory)
    : ClickHouseContextFactory<OlapDbContext>
{
    public static string HttpClientName => "ClickHouseClient";

    private string _connectionString = configuration.GetConnectionString("clickhouse-observability-db")
                                                 ?? throw new ArgumentNullException(
                                                     "clickhouse-observability-db", "Connection string 'clickhouse-observability-db' is not configured.");

    protected override void SetupContextOptions(ClickHouseContextOptionsBuilder<OlapDbContext> optionsBuilder)
    {
        optionsBuilder
            .WithConnectionString(_connectionString)
            .WithHttpClientFactory(httpClientFactory, HttpClientName);
    }
}