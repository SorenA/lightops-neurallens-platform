using LightOps.NeuralLens.Component.ServiceDefaults;
using LightOps.NeuralLens.EvaluationApi.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddRuntimeServices();
builder.AddRuntimeOpenApiSpecification();
builder.AddRuntimeAuth();
builder.AddRuntimeApiVersioning();

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