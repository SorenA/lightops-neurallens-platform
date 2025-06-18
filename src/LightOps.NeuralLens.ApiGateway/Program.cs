using LightOps.NeuralLens.ApiGateway.Extensions;
using LightOps.NeuralLens.Component.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddRuntimeServices();

// Add other services to the container
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver();
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
}

app.MapReverseProxy();
app.MapControllers();

app.Run();