using System.ClientModel;
using System.ClientModel.Primitives;
using System.Reflection;
using Microsoft.OpenApi.Models;
using OpenAI;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    return new OpenAIClient(
        new ApiKeyCredential(config.GetValue<string>("OpenAI:ApiKey")!),
        new OpenAIClientOptions
        {
            Endpoint = new Uri(config.GetValue<string>("OpenAI:Endpoint")!),
            NetworkTimeout = TimeSpan.FromMinutes(5),
            ClientLoggingOptions = new ClientLoggingOptions()
            {
                EnableLogging = config.GetValue<bool>("OpenAI:EnableLogging")!,
                EnableMessageContentLogging = config.GetValue<bool>("OpenAI:EnableLogging")!,
                EnableMessageLogging = config.GetValue<bool>("OpenAI:EnableLogging")!,
            },
        });
});

// Log to console - not connected to Aspire
builder.Logging.AddConsole().AddOpenTelemetry(logger =>
{
    // Export to AppHost
    logger.AddOtlpExporter();
});

// Enable model diagnostics with sensitive data.
AppContext.SetSwitch("Microsoft.SemanticKernel.Experimental.GenAI.EnableOTelDiagnosticsSensitive", true);

// Export OpenTelemetry traces to Ingest API
var ingestApiBase = Environment.GetEnvironmentVariable("services__ingest-api__https__0")!;
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resourceBuilder => resourceBuilder
        .AddService("Sample.SemanticKernel.WebApi", serviceVersion: "0.0.1")
        .AddAttributes(new Dictionary<string, object>
        {
            ["deployment.environment"] = "local",
        }))
    .WithTracing(tracing =>
    {
        tracing.AddSource(builder.Environment.ApplicationName)
            .AddSource("Microsoft.SemanticKernel*")
            .AddSource("OpenAI.*")
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();

        // Export to console
        tracing.AddConsoleExporter();

        // Export to ingest API
        tracing.AddOtlpExporter(options =>
            {
                options.Protocol = OtlpExportProtocol.Grpc;
                options.Endpoint = new Uri(ingestApiBase);
            });

        // Export to AppHost
        tracing.AddOtlpExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation();

        // Export to AppHost
        metrics.AddOtlpExporter();
    });
    //.UseOtlpExporter(OtlpExportProtocol.HttpProtobuf, new Uri("https://ingest-api/otlp"));

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "Sample API",
            Description = "A sample Web API with OpenTelemetry instrumentation.",
            Version = "v1",
        };

        return Task.CompletedTask;
    });
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAll");
    app.MapOpenApi();
    app.MapScalarApiReference("/");
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();