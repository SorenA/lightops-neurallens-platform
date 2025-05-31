using LightOps.NeuralLens.Component.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers();
var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = string.Empty;

        var organizationApiBase = Environment.GetEnvironmentVariable("services__organization-api__https__0");
        options.SwaggerEndpoint($"{organizationApiBase}/openapi/v1.json", "Organization API v1");

        var workspaceApiBase = Environment.GetEnvironmentVariable("services__workspace-api__https__0");
        options.SwaggerEndpoint($"{workspaceApiBase}/openapi/v1.json", "Workspace API v1");

        var observabilityApiBase = Environment.GetEnvironmentVariable("services__observability-api__https__0");
        options.SwaggerEndpoint($"{observabilityApiBase}/openapi/v1.json", "Observability API v1");

        var evaluationApiBase = Environment.GetEnvironmentVariable("services__evaluation-api__https__0");
        options.SwaggerEndpoint($"{evaluationApiBase}/openapi/v1.json", "Evaluation API v1");

        var ingestApiBase = Environment.GetEnvironmentVariable("services__ingest-api__https__0");
        options.SwaggerEndpoint($"{ingestApiBase}/openapi/v1.json", "Ingest API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
