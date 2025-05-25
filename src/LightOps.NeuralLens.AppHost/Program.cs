using Google.Protobuf.WellKnownTypes;

var builder = DistributedApplication.CreateBuilder(args);

// Add databases
var mongo = builder.AddMongoDB("mongo", 27017)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataBindMount("./../../data/mongo")
    .WithMongoExpress(expressBuilder => expressBuilder.WithHostPort(20590));
var mongoOrganizationDb = mongo.AddDatabase("mongo-organization-db", "organization_db");
var mongoWorkspaceDb = mongo.AddDatabase("mongo-workspace-db", "workspace_db");
var mongoObservabilityDb = mongo.AddDatabase("mongo-observability-db", "observability_db");
var mongoEvaluationDb = mongo.AddDatabase("mongo-evaluation-db", "evaluation_db");

// Add API services
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
    .WithReference(workspaceApi)
    .WithReference(observabilityApi)
    .WithReference(evaluationApi)
    .WaitFor(organizationApi)
    .WaitFor(workspaceApi)
    .WaitFor(observabilityApi)
    .WaitFor(evaluationApi);

builder
    .AddProject<Projects.LightOps_NeuralLens_Frontend_OpenAPI>("frontend-openapi")
    .WithExternalHttpEndpoints()
    .WithReference(organizationApi)
    .WithReference(workspaceApi)
    .WithReference(observabilityApi)
    .WithReference(evaluationApi)
    .WaitFor(organizationApi)
    .WaitFor(workspaceApi)
    .WaitFor(observabilityApi)
    .WaitFor(evaluationApi);

builder.Build().Run();
