using LightOps.NeuralLens.AppHost;
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Add databases
var mongo = builder.AddMongoDB("mongo", 27017)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataBindMount("./../../data/mongo");
var mongoAuthDb = mongo.AddDatabase("mongo-auth-db", "auth_db");
var mongoEvaluationDb = mongo.AddDatabase("mongo-evaluation-db", "evaluation_db");
var mongoObservabilityDb = mongo.AddDatabase("mongo-observability-db", "observability_db");
var mongoOrganizationDb = mongo.AddDatabase("mongo-organization-db", "organization_db");
var mongoPermissionDb = mongo.AddDatabase("mongo-permission-db", "permission_db");
var mongoWorkspaceDb = mongo.AddDatabase("mongo-workspace-db", "workspace_db");

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
    .WithReference(clickhouse).WaitFor(clickhouse)
    .WithExternalHttpEndpoints();

// Create auth encryption key and client secrets
var authApiClientSecret = builder.AddParameter("auth-api-client-secret", true);
var evaluationApiClientSecret = builder.AddParameter("evaluation-api-client-secret", true);
var ingestApiClientSecret = builder.AddParameter("ingest-api-client-secret", true);
var observabilityApiClientSecret = builder.AddParameter("observability-api-client-secret", true);
var organizationApiClientSecret = builder.AddParameter("organization-api-client-secret", true);
var permissionApiClientSecret = builder.AddParameter("permission-api-client-secret", true);
var workspaceApiClientSecret = builder.AddParameter("workspace-api-client-secret", true);

// Add API gateway
var apiGateway = builder
    .AddProject<Projects.LightOps_NeuralLens_ApiGateway>("api-gateway")
    .WithExternalHttpEndpoints();

// Add API services
var authApi = builder
    .AddProject<Projects.LightOps_NeuralLens_AuthApi>("auth-api")
    .WithReference(mongoAuthDb).WaitFor(mongoAuthDb)
    .WithReference(apiGateway)
    .WithEnvironment("Services__auth-api__ClientSecret", authApiClientSecret)
    .WithEnvironment("Services__evaluation-api__ClientSecret", evaluationApiClientSecret)
    .WithEnvironment("Services__ingest-api__ClientSecret", ingestApiClientSecret)
    .WithEnvironment("Services__observability-api__ClientSecret", observabilityApiClientSecret)
    .WithEnvironment("Services__organization-api__ClientSecret", organizationApiClientSecret)
    .WithEnvironment("Services__permission-api__ClientSecret", permissionApiClientSecret)
    .WithEnvironment("Services__workspace-api__ClientSecret", workspaceApiClientSecret);
var evaluationApi = builder
    .AddProject<Projects.LightOps_NeuralLens_EvaluationApi>("evaluation-api")
    .WithReference(mongoEvaluationDb).WaitFor(mongoEvaluationDb)
    .WithReference(apiGateway)
    .WithEnvironment("AuthManagedIdentity__ClientSecret", evaluationApiClientSecret);
var ingestApi = builder
    .AddProject<Projects.LightOps_NeuralLens_IngestApi>("ingest-api")
    .WithReference(clickhouseObservabilityDb).WaitFor(clickhouseObservabilityDb)
    .WithReference(apiGateway)
    .WithEnvironment("AuthManagedIdentity__ClientSecret", ingestApiClientSecret);
var observabilityApi = builder
    .AddProject<Projects.LightOps_NeuralLens_ObservabilityApi>("observability-api")
    .WithReference(mongoObservabilityDb).WaitFor(mongoObservabilityDb)
    .WithReference(apiGateway)
    .WithEnvironment("AuthManagedIdentity__ClientSecret", observabilityApiClientSecret);
var organizationApi = builder
    .AddProject<Projects.LightOps_NeuralLens_OrganizationApi>("organization-api")
    .WithReference(mongoOrganizationDb).WaitFor(mongoOrganizationDb)
    .WithReference(apiGateway)
    .WithEnvironment("AuthManagedIdentity__ClientSecret", organizationApiClientSecret);
var permissionApi = builder
    .AddProject<Projects.LightOps_NeuralLens_PermissionApi>("permission-api")
    .WithReference(mongoPermissionDb).WaitFor(mongoPermissionDb)
    .WithReference(apiGateway)
    .WithEnvironment("AuthManagedIdentity__ClientSecret", permissionApiClientSecret);
var workspaceApi = builder
    .AddProject<Projects.LightOps_NeuralLens_WorkspaceApi>("workspace-api")
    .WithReference(mongoWorkspaceDb).WaitFor(mongoWorkspaceDb)
    .WithReference(apiGateway)
    .WithEnvironment("AuthManagedIdentity__ClientSecret", workspaceApiClientSecret);

// Add frontend services
var managementFrontend = builder.AddTurboRepoProject(
        "management-frontend",
        "../LightOps.NeuralLens.Frontends/apps/management",
        scriptName: "dev")
    .WithHttpsEndpoint(20603, env: "PORT")
    .WithExternalHttpEndpoints()
    .WithEnvironment("NODE_TLS_REJECT_UNAUTHORIZED", builder.Environment.IsDevelopment() ? "0" : "1")
    .WithEnvironment("NEXT_PUBLIC_AUTH_API_URL", authApi.GetEndpoint("https"))
    .WithEnvironment("NEXT_PUBLIC_EVALUATION_API_URL", evaluationApi.GetEndpoint("https"))
    .WithEnvironment("NEXT_PUBLIC_OBSERVABILITY_API_URL", observabilityApi.GetEndpoint("https"))
    .WithEnvironment("NEXT_PUBLIC_ORGANIZATION_API_URL", organizationApi.GetEndpoint("https"))
    .WithEnvironment("NEXT_PUBLIC_WORKSPACE_API_URL", workspaceApi.GetEndpoint("https"));

var documentationFrontend = builder
    .AddProject<Projects.LightOps_NeuralLens_DocumentationFrontend>("documentation-frontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiGateway);

// Add sample runtimes
builder
    .AddProject<Projects.Sample_SemanticKernel_WebApi>("sample-semantickernel-api")
    .WithReference(ingestApi).WaitFor(ingestApi);

// Add references to Auth API
authApi
    .WithReference(managementFrontend)
    .WithReference(documentationFrontend);

// Add references to API Gateway
apiGateway
    .WithReference(authApi)
    .WithReference(evaluationApi)
    .WithReference(ingestApi)
    .WithReference(observabilityApi)
    .WithReference(organizationApi)
    .WithReference(permissionApi)
    .WithReference(workspaceApi);

builder.Build().Run();
