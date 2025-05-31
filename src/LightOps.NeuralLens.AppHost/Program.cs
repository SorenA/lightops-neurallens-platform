using Aspire.Hosting;
using Google.Protobuf.WellKnownTypes;

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

// Add API services
var ingestApi = builder
    .AddProject<Projects.LightOps_NeuralLens_IngestApi>("ingest-api")
    .WithReference(redis)
    .WaitFor(redis);
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

builder.Build().Run();
