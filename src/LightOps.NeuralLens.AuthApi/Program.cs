using LightOps.NeuralLens.AuthApi.Extensions;
using LightOps.NeuralLens.AuthApi.Workers;
using LightOps.NeuralLens.Component.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<ScopeMigrationWorker>();
builder.Services.AddHostedService<ApplicationMigrationWorker>();
builder.AddServiceDefaults();
builder.AddRuntimeServices();
builder.AddRuntimeOpenApiSpecification();
builder.AddRuntimeAuth();
builder.AddRuntimeApiVersioning();

// Add databases
builder.AddMongoDBClient(connectionName: "mongo-auth-db");

// Add services to the container.
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

app.UseForwardedHeaders();
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