using System.Reflection;
using ClickHouse.Facades.Migrations;

namespace LightOps.NeuralLens.ClickHouseMigrationWorker.Domain;

public class NeuralLensClickHouseMigrationsLocator : ClickHouseAssemblyMigrationsLocator
{
    protected override Assembly TargetAssembly => typeof(NeuralLensClickHouseMigrationsLocator).Assembly;
}