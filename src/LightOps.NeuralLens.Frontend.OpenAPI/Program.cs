using LightOps.NeuralLens.Component.ServiceDefaults;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers();
var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    var organizationApiBase = builder.Configuration.GetValue<string>("Services:organization-api:Https:0");
    var workspaceApiBase = builder.Configuration.GetValue<string>("Services:workspace-api:Https:0");
    var observabilityApiBase = builder.Configuration.GetValue<string>("Services:observability-api:Https:0");
    var evaluationApiBase = builder.Configuration.GetValue<string>("Services:evaluation-api:Https:0");
    var ingestApiBase = builder.Configuration.GetValue<string>("Services:ingest-api:Https:0");
    var authApiBase = builder.Configuration.GetValue<string>("Services:auth-api:Https:0");

    app.MapScalarApiReference("/", options =>
    {
        options.AddDocument("Organization API V1", routePattern: $"{organizationApiBase}/openapi/v1.json");
        options.AddDocument("Workspace API V1", routePattern: $"{workspaceApiBase}/openapi/v1.json");
        options.AddDocument("Observability API V1", routePattern: $"{observabilityApiBase}/openapi/v1.json");
        options.AddDocument("Evaluation API V1", routePattern: $"{evaluationApiBase}/openapi/v1.json");
        options.AddDocument("Ingest API V1", routePattern: $"{ingestApiBase}/openapi/v1.json");
        options.AddDocument("Auth API V1", routePattern: $"{authApiBase}/openapi/v1.json");

        options.AddPreferredSecuritySchemes(["OAuth2"]);

        options.WithPersistentAuthentication();
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
