using System.Reflection;
using ClickHouse.Facades.Migrations;

namespace LightOps.NeuralLens.ClickHouseMigrationWorker.Domain;

public class OlapMigrationsLocator : ClickHouseAssemblyMigrationsLocator
{
    protected override Assembly TargetAssembly => typeof(OlapMigrationsLocator).Assembly;
}