using ClickHouse.Facades;

namespace LightOps.NeuralLens.ClickHouseMigrationWorker.Domain;

public class NeuralLensClickHouseDbContext : ClickHouseContext<NeuralLensClickHouseDbContext>
{
    public ObservabilityFacade ObservabilityFacade => GetFacade<ObservabilityFacade>();
}