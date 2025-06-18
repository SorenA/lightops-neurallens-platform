using System.Net;
using ClickHouse.Facades;
using LightOps.DependencyInjection.Configuration;
using LightOps.Mapping.Configuration;
using LightOps.NeuralLens.Component.ServiceDefaults;
using LightOps.NeuralLens.IngestApi.Domain.DbContexts;
using LightOps.NeuralLens.IngestApi.Extensions;
using LightOps.NeuralLens.IngestApi.GrpcServices;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddRuntimeServices();
builder.AddRuntimeOpenApiSpecification();
builder.AddRuntimeAuth();
builder.AddRuntimeApiVersioning();

// Add ClickHouse contexts
builder.Services.AddHttpClient(OlapDbContextFactory.HttpClientName)
    .ConfigureHttpClient((_, httpClient) =>
    {
        httpClient.Timeout = TimeSpan.FromSeconds(60);
    })
    .SetHandlerLifetime(Timeout.InfiniteTimeSpan)
    .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
    {
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
        MaxConnectionsPerServer = 8,
    });
builder.Services.AddClickHouseContext<OlapDbContext, OlapDbContextFactory>(
    serviceBuilder =>
    {
        serviceBuilder.AddFacade<TraceFacade>();
        serviceBuilder.AddFacade<SpanFacade>();
        serviceBuilder.AddFacade<EventFacade>();
    });

// Add services to the container.
builder.Services.AddLightOpsDependencyInjection(root =>
{
    root.AddMapping();
});
builder.Services.AddGrpc(o => o.EnableDetailedErrors = true)
    .AddJsonTranscoding();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
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

app.UseHttpsRedirection();
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAll");
    app.UseRuntimeOpenApiSpecification();
}

app.UseRuntimeAuth();
app.MapGrpcService<OpenTelemetryCollectorTraceGrpcService>();
app.MapControllers();

app.Run();