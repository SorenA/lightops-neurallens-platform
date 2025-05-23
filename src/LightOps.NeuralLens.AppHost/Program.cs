using Google.Protobuf.WellKnownTypes;

var builder = DistributedApplication.CreateBuilder(args);

// Add API services
var organizationApi = builder
    .AddProject<Projects.LightOps_NeuralLens_OrganizationApi>("organization-api");
var projectApi = builder
    .AddProject<Projects.LightOps_NeuralLens_ProjectApi>("project-api");
var observabilityApi = builder
    .AddProject<Projects.LightOps_NeuralLens_ObservabilityApi>("observability-api");
var evaluationApi = builder
    .AddProject<Projects.LightOps_NeuralLens_EvaluationApi>("evaluation-api");

// Add frontend services
builder
    .AddProject<Projects.LightOps_NeuralLens_Frontend_Management>("frontend-management")
    .WithExternalHttpEndpoints()
    .WithReference(organizationApi)
    .WithReference(projectApi)
    .WithReference(observabilityApi)
    .WithReference(evaluationApi)
    .WaitFor(organizationApi)
    .WaitFor(projectApi)
    .WaitFor(observabilityApi)
    .WaitFor(evaluationApi);

builder
    .AddProject<Projects.LightOps_NeuralLens_Frontend_OpenAPI>("frontend-openapi")
    .WithExternalHttpEndpoints()
    .WithReference(organizationApi)
    .WithReference(projectApi)
    .WithReference(observabilityApi)
    .WithReference(evaluationApi)
    .WaitFor(organizationApi)
    .WaitFor(projectApi)
    .WaitFor(observabilityApi)
    .WaitFor(evaluationApi);

builder.Build().Run();
