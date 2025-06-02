using System.ClientModel;
using System.ClientModel.Primitives;
using System.Reflection;
using Microsoft.OpenApi.Models;
using OpenAI;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

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
builder.Logging.AddConsole().AddOpenTelemetry();

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
        tracing.AddConsoleExporter()
            .AddOtlpExporter(options =>
            {
                options.Protocol = OtlpExportProtocol.Grpc;
                options.Endpoint = new Uri(ingestApiBase);
            });
    });
    //.UseOtlpExporter(OtlpExportProtocol.HttpProtobuf, new Uri("https://ingest-api/otlp"));

// Export OpenTelemetry traces to ingest API
/*builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .ConfigureResource(resourceBuilder => resourceBuilder
            .AddService("Sample.SemanticKernel.WebApi", serviceVersion: "0.0.1")
            .AddAttributes(new Dictionary<string, object>
            {
                ["environment.name"] = "wat",
            })
            .AddEnvironmentVariableDetector())
        .AddSource("Microsoft.SemanticKernel*")
        .AddAspNetCoreInstrumentation()
        .AddConsoleExporter()
        .AddOtlpExporter(options =>
        {
            options.Protocol = OtlpExportProtocol.HttpProtobuf;
            options.Endpoint = new Uri("https://ingest-api/otlp/v1/traces");
        }));*/

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sample API",
        Description = "A sample Web API with OpenTelemetry instrumentation.",
        Version = "v1",
    });

    // Add comments to the generated Swagger JSON
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();