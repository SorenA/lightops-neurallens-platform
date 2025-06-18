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
    var apiGatewayBase = builder.Configuration.GetValue<string>("Services:api-gateway:Https:0")!;

    _ = app.MapScalarApiReference("/api", options =>
    {
        options.AddDocument("Auth API V1", routePattern: $"{apiGatewayBase}/openapi/auth-api/v1.json");
        options.AddDocument("Evaluation API V1", routePattern: $"{apiGatewayBase}/openapi/evaluation-api/v1.json");
        options.AddDocument("Ingest API V1", routePattern: $"{apiGatewayBase}/openapi/ingest-api/v1.json");
        options.AddDocument("Observability API V1", routePattern: $"{apiGatewayBase}/openapi/observability-api/v1.json");
        options.AddDocument("Organization API V1", routePattern: $"{apiGatewayBase}/openapi/organization-api/v1.json");
        options.AddDocument("Permission API V1", routePattern: $"{apiGatewayBase}/openapi/permission-api/v1.json");
        options.AddDocument("Workspace API V1", routePattern: $"{apiGatewayBase}/openapi/workspace-api/v1.json");

        options.AddPreferredSecuritySchemes(["OAuth2"]);
        options.WithPersistentAuthentication();

        // Override servers
        options.Servers = new List<ScalarServer>()
        {
            new(apiGatewayBase, "API Gateway")
        };
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", ctx =>
{
    // Redirect to the API documentation
    ctx.Response.Redirect("/api", false);
    return Task.CompletedTask;
});

app.Run();
