using ClickHouse.Facades;

namespace LightOps.NeuralLens.IngestApi.Domain.DbContexts;

public class OlapDbContext : ClickHouseContext<OlapDbContext>
{
    public static string ObservabilityDb => "observability_db";

    public TraceFacade Traces => GetFacade<TraceFacade>();
    public SpanFacade Spans => GetFacade<SpanFacade>();
    public EventFacade Events => GetFacade<EventFacade>();
}