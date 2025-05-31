using System.Reflection;
using LightOps.NeuralLens.Component.ServiceDefaults;
using LightOps.NeuralLens.IngestApi.GrpcServices;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddGrpc(o => o.EnableDetailedErrors = true)
    .AddJsonTranscoding();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddGrpcSwagger();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ingest API",
        Description = "A Web API for ingest as part of the LightOps NeuralLens Platform.",
        Version = "v1",
    });

    options.CustomSchemaIds(x => x.FullName?.Replace("+", "."));

    // Add comments to the generated Swagger JSON
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    options.IncludeXmlComments(xmlFilePath);
    options.IncludeGrpcXmlComments(xmlFilePath, includeControllerXmlComments: true);
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        cors =>
            cors.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
});
var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAll");
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "openapi/{documentName}.json";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapGrpcService<OpenTelemetryCollectorTraceGrpcService>();
app.MapControllers();

app.Run();