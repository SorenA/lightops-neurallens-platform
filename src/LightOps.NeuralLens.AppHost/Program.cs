var builder = DistributedApplication.CreateBuilder(args);

// Add databases
var mongo = builder.AddMongoDB("mongo", 27017)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataBindMount("./../../data/mongo");
var mongoOrganizationDb = mongo.AddDatabase("mongo-organization-db", "organization_db");
var mongoWorkspaceDb = mongo.AddDatabase("mongo-workspace-db", "workspace_db");
var mongoObservabilityDb = mongo.AddDatabase("mongo-observability-db", "observability_db");
var mongoEvaluationDb = mongo.AddDatabase("mongo-evaluation-db", "evaluation_db");

var redis = builder.AddRedis("redis", 6379)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataBindMount("./../../data/redis")
    .WithPersistence(
        interval: TimeSpan.FromSeconds(30),
        keysChangedThreshold: 10);

var clickhouse = builder.AddClickHouse("clickhouse", port: 8123)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataBindMount("./../../data/clickhouse")
    .WithImageTag("25.5");
var clickhouseObservabilityDb = clickhouse.AddDatabase("clickhouse-observability-db", "observability_db");
var clickhouseEvaluationDb = clickhouse.AddDatabase("clickhouse-evaluation-db", "evaluation_db");
var clickHouseUi = builder
    .AddContainer("clickhouse-ui", "ghcr.io/caioricciuti/ch-ui")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithExternalHttpEndpoints()
    .WithHttpEndpoint(5521, 5521)
    .WithEnvironment("VITE_CLICKHOUSE_URL", "http://localhost:8123")
    .WithEnvironment("VITE_CLICKHOUSE_USER", "default")
    .WithEnvironment("VITE_CLICKHOUSE_PASS", clickhouse.Resource.PasswordParameter.Value);

// Add worker services
var clickhouseMigrationWorker = builder
    .AddProject<Projects.LightOps_NeuralLens_ClickHouseMigrationWorker>("clickhouse-migration-worker")
    .WithReference(clickhouse)
    .WaitFor(clickhouse);

// Add API services
var ingestApi = builder
    .AddProject<Projects.LightOps_NeuralLens_IngestApi>("ingest-api")
    .WithReference(clickhouseObservabilityDb)
    .WaitFor(clickhouseObservabilityDb);
var organizationApi = builder
    .AddProject<Projects.LightOps_NeuralLens_OrganizationApi>("organization-api")
    .WithReference(mongoOrganizationDb)
    .WaitFor(mongoOrganizationDb);
var workspaceApi = builder
    .AddProject<Projects.LightOps_NeuralLens_WorkspaceApi>("workspace-api")
    .WithReference(mongoWorkspaceDb)
    .WaitFor(mongoWorkspaceDb);
var observabilityApi = builder
    .AddProject<Projects.LightOps_NeuralLens_ObservabilityApi>("observability-api")
    .WithReference(mongoObservabilityDb)
    .WaitFor(mongoObservabilityDb);
var evaluationApi = builder
    .AddProject<Projects.LightOps_NeuralLens_EvaluationApi>("evaluation-api")
    .WithReference(mongoEvaluationDb)
    .WaitFor(mongoEvaluationDb);

// Add frontend services
builder
    .AddProject<Projects.LightOps_NeuralLens_Frontend_Management>("frontend-management")
    .WithExternalHttpEndpoints()
    .WithReference(organizationApi)
    .WaitFor(organizationApi)
    .WithReference(workspaceApi)
    .WaitFor(workspaceApi)
    .WithReference(observabilityApi)
    .WaitFor(observabilityApi)
    .WithReference(evaluationApi)
    .WaitFor(evaluationApi);

builder
    .AddProject<Projects.LightOps_NeuralLens_Frontend_OpenAPI>("frontend-openapi")
    .WithExternalHttpEndpoints()
    .WithReference(organizationApi)
    .WaitFor(organizationApi)
    .WithReference(workspaceApi)
    .WaitFor(workspaceApi)
    .WithReference(observabilityApi)
    .WaitFor(observabilityApi)
    .WithReference(evaluationApi)
    .WaitFor(evaluationApi)
    .WithReference(ingestApi)
    .WaitFor(ingestApi);

// Add sample runtimes
builder
    .AddProject<Projects.Sample_SemanticKernel_WebApi>("sample-semantickernel-api")
    .WithReference(ingestApi)
    .WaitFor(ingestApi);

builder.Build().Run();
