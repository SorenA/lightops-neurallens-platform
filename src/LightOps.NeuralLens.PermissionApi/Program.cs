using LightOps.DependencyInjection.Configuration;
using LightOps.Mapping.Configuration;
using LightOps.NeuralLens.Component.ServiceDefaults;
using LightOps.NeuralLens.PermissionApi.Extensions;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddRuntimeServices();
builder.AddRuntimeOpenApiSpecification();
builder.AddRuntimeAuth();

// Add databases
builder.AddMongoDBClient(connectionName: "mongo-permission-db");

// Add other services to the container
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddLightOpsDependencyInjection(root =>
{
    root.AddMapping();
});
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
app.MapControllers();

app.Run();