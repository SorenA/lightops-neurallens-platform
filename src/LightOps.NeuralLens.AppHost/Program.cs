var builder = DistributedApplication.CreateBuilder(args);

// Add databases
var mongo = builder.AddMongoDB("mongo", 27017)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataBindMount("./../../data/mongo");
var mongoAuthDb = mongo.AddDatabase("mongo-auth-db", "auth_db");
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

var clickhousePassword = builder.AddParameter("clickhouse-password", true);
var clickhouse = builder.AddClickHouse("clickhouse", port: 8123, password: clickhousePassword)
    .WithLifetime(ContainerLifetime.Persistent)
    /*.WithDataBindMount("./../../data/clickhouse")*/
    .WithDataVolume("clickhouse") // Temporarily use named volume while bind mount bug is not fixed
    .WithImageTag("25.5")
    .WithExternalHttpEndpoints();
var clickhouseObservabilityDb = clickhouse.AddDatabase("clickhouse-observability-db", "observability_db");
var clickhouseEvaluationDb = clickhouse.AddDatabase("clickhouse-evaluation-db", "evaluation_db");
var clickHouseUi = builder
    .AddContainer("clickhouse-ui", "ghcr.io/caioricciuti/ch-ui")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithExternalHttpEndpoints()
    .WithHttpEndpoint(5521, 5521)
    .WithEnvironment("VITE_CLICKHOUSE_URL", "http://localhost:8123")
    .WithEnvironment("VITE_CLICKHOUSE_USER", "default")
    .WithEnvironment("VITE_CLICKHOUSE_PASS", clickhousePassword);

// Add worker services
var clickhouseMigrationWorker = builder
    .AddProject<Projects.LightOps_NeuralLens_ClickHouseMigrationWorker>("clickhouse-migration-worker")
    .WithReference(clickhouse).WaitFor(clickhouse);

// Create auth encryption key and client secrets
var authApiClientSecret = builder.AddParameter("auth-api-client-secret", true);
var evaluationApiClientSecret = builder.AddParameter("evaluation-api-client-secret", true);
var ingestApiClientSecret = builder.AddParameter("ingest-api-client-secret", true);
var observabilityApiClientSecret = builder.AddParameter("observability-api-client-secret", true);
var organizationApiClientSecret = builder.AddParameter("organization-api-client-secret", true);
var workspaceApiClientSecret = builder.AddParameter("workspace-api-client-secret", true);
var frontendManagementClientSecret = builder.AddParameter("frontend-management-client-secret", true);

// Add API services
var authApi = builder
    .AddProject<Projects.LightOps_NeuralLens_AuthApi>("auth-api")
    .WithReference(mongoAuthDb).WaitFor(mongoAuthDb)
    .WithEnvironment("Services__auth-api__ClientSecret", authApiClientSecret)
    .WithEnvironment("Auth__Clients__EvaluationApi__ClientSecret", evaluationApiClientSecret)
    .WithEnvironment("Auth__Clients__IngestApi__ClientSecret", ingestApiClientSecret)
    .WithEnvironment("Auth__Clients__ObservabilityApi__ClientSecret", observabilityApiClientSecret)
    .WithEnvironment("Auth__Clients__OrganizationApi__ClientSecret", organizationApiClientSecret)
    .WithEnvironment("Auth__Clients__WorkspaceApi__ClientSecret", workspaceApiClientSecret)
    .WithEnvironment("Auth__Clients__FrontendManagement__ClientSecret", frontendManagementClientSecret);
var evaluationApi = builder
    .AddProject<Projects.LightOps_NeuralLens_EvaluationApi>("evaluation-api")
    .WithReference(mongoEvaluationDb).WaitFor(mongoEvaluationDb)
    .WithReference(authApi)
    .WithEnvironment("Services__auth-api__ClientSecret", evaluationApiClientSecret);
var ingestApi = builder
    .AddProject<Projects.LightOps_NeuralLens_IngestApi>("ingest-api")
    .WithReference(clickhouseObservabilityDb).WaitFor(clickhouseObservabilityDb)
    .WithReference(authApi)
    .WithEnvironment("Services__auth-api__ClientSecret", ingestApiClientSecret);
var observabilityApi = builder
    .AddProject<Projects.LightOps_NeuralLens_ObservabilityApi>("observability-api")
    .WithReference(mongoObservabilityDb).WaitFor(mongoObservabilityDb)
    .WithReference(authApi)
    .WithEnvironment("Services__auth-api__ClientSecret", observabilityApiClientSecret);
var organizationApi = builder
    .AddProject<Projects.LightOps_NeuralLens_OrganizationApi>("organization-api")
    .WithReference(mongoOrganizationDb).WaitFor(mongoOrganizationDb)
    .WithReference(authApi)
    .WithEnvironment("Services__auth-api__ClientSecret", organizationApiClientSecret);
var workspaceApi = builder
    .AddProject<Projects.LightOps_NeuralLens_WorkspaceApi>("workspace-api")
    .WithReference(mongoWorkspaceDb).WaitFor(mongoWorkspaceDb)
    .WithReference(authApi)
    .WithEnvironment("Services__auth-api__ClientSecret", workspaceApiClientSecret);

// Add frontend services
var managementFrontend = builder
    .AddProject<Projects.LightOps_NeuralLens_Frontend_Management>("frontend-management")
    .WithExternalHttpEndpoints()
    .WithReference(authApi)
    .WithReference(evaluationApi)
    .WithReference(observabilityApi)
    .WithReference(organizationApi).WaitFor(organizationApi)
    .WithReference(workspaceApi).WaitFor(workspaceApi)
    .WithEnvironment("Services__auth-api__ClientSecret", frontendManagementClientSecret);

builder
    .AddProject<Projects.LightOps_NeuralLens_Frontend_OpenAPI>("frontend-openapi")
    .WithExternalHttpEndpoints()
    .WithReference(authApi)
    .WithReference(evaluationApi)
    .WithReference(ingestApi)
    .WithReference(observabilityApi)
    .WithReference(organizationApi)
    .WithReference(workspaceApi);

// Add sample runtimes
builder
    .AddProject<Projects.Sample_SemanticKernel_WebApi>("sample-semantickernel-api")
    .WithReference(ingestApi).WaitFor(ingestApi);

// Add references to Auth API
authApi.WithReference(organizationApi)
    .WithReference(workspaceApi)
    .WithReference(managementFrontend);

builder.Build().Run();
