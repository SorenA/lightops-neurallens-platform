using LightOps.NeuralLens.AuthApi;
using LightOps.NeuralLens.AuthApi.Extensions;
using LightOps.NeuralLens.Component.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<SeedWorker>();
builder.AddServiceDefaults();
builder.AddRuntimeServices();
builder.AddRuntimeOpenApiSpecification();
builder.AddRuntimeAuth();

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