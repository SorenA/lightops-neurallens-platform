using System.Reflection;
using LightOps.NeuralLens.AuthApi;
using LightOps.NeuralLens.Component.ServiceDefaults;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<SeedWorker>();
builder.AddServiceDefaults();

// Add databases
builder.AddMongoDBClient(connectionName: "mongo-auth-db");

// Add authentication
builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseMongoDb();
    })
    .AddServer(options =>
    {
        // Enable the authorization, introspection and token endpoints.
        options.SetAuthorizationEndpointUris("authorize")
            .SetIntrospectionEndpointUris("introspect")
            .SetTokenEndpointUris("token");

        // Enable authorization code and refresh token flows.
        options.AllowAuthorizationCodeFlow()
            .AllowRefreshTokenFlow()
            .AllowClientCredentialsFlow();

        if (builder.Environment.IsDevelopment())
        {
            // Register the signing and encryption credentials.
            options.AddDevelopmentEncryptionCertificate()
                .AddDevelopmentSigningCertificate();
        }

        options.UseAspNetCore();
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

// Add services to the container.
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Auth API",
        Description = "A Web API for auth as part of the LightOps NeuralLens Platform.",
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

app.UseForwardedHeaders();
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
app.UseAuthorization();
app.MapControllers();

app.Run();