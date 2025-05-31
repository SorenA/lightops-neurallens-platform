using System.Reflection;
using FluentValidation;
using LightOps.DependencyInjection.Configuration;
using LightOps.Mapping.Api.Mappers;
using LightOps.Mapping.Configuration;
using LightOps.NeuralLens.Component.ServiceDefaults;
using LightOps.NeuralLens.WorkspaceApi.Domain.Mappers;
using LightOps.NeuralLens.WorkspaceApi.Domain.Models;
using LightOps.NeuralLens.WorkspaceApi.Domain.Repositories;
using LightOps.NeuralLens.WorkspaceApi.Domain.RequestValidators;
using LightOps.NeuralLens.WorkspaceApi.Domain.Services;
using LightOps.NeuralLens.WorkspaceApi.Models;
using LightOps.NeuralLens.WorkspaceApi.Requests;
using Microsoft.OpenApi.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Add databases
builder.AddMongoDBClient(connectionName: "mongo-workspace-db");

// Add repositories
builder.Services.AddTransient<IWorkspaceRepository, MongoWorkspaceRepository>();

// Add services
builder.Services.AddTransient<WorkspaceService>();
builder.Services.AddTransient<IngestKeyService>();

// Add mappers
builder.Services.AddTransient<IMapper<Workspace, WorkspaceViewModel>, WorkspaceViewModelMapper>();

// Add validators
builder.Services.AddScoped<IValidator<CreateWorkspaceRequest>, CreateWorkspaceRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateWorkspaceRequest>, UpdateWorkspaceRequestValidator>();

// Add other services to the container
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddLightOpsDependencyInjection(root =>
{
    root.AddMapping();
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Workspace API",
        Description = "A Web API for workspaces as part of the LightOps NeuralLens Platform.",
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
app.MapControllers();

app.Run();