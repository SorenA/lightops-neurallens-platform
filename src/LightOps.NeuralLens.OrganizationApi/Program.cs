using System.Reflection;
using FluentValidation;
using LightOps.DependencyInjection.Configuration;
using LightOps.Mapping.Api.Mappers;
using LightOps.Mapping.Configuration;
using LightOps.NeuralLens.Component.ServiceDefaults;
using LightOps.NeuralLens.OrganizationApi.Domain.Mappers;
using LightOps.NeuralLens.OrganizationApi.Domain.Models;
using LightOps.NeuralLens.OrganizationApi.Domain.Repositories;
using LightOps.NeuralLens.OrganizationApi.Domain.RequestValidators;
using LightOps.NeuralLens.OrganizationApi.Domain.Services;
using LightOps.NeuralLens.OrganizationApi.Models;
using LightOps.NeuralLens.OrganizationApi.Requests;
using Microsoft.OpenApi.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Add databases
builder.AddMongoDBClient(connectionName: "mongo-organization-db");

// Add repositories
builder.Services.AddTransient<IOrganizationRepository, MongoOrganizationRepository>();

// Add services
builder.Services.AddTransient<OrganizationService>();

// Add mappers
builder.Services.AddTransient<IMapper<Organization, OrganizationViewModel>, OrganizationViewModelMapper>();

// Add validators
builder.Services.AddScoped<IValidator<CreateOrganizationRequest>, CreateOrganizationRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateOrganizationRequest>, UpdateOrganizationRequestValidator>();

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
        Title = "Organization API",
        Description = "A Web API for organizations as part of the LightOps NeuralLens Platform.",
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