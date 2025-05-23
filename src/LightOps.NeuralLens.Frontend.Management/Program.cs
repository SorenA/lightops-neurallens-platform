using LightOps.NeuralLens.Component.EvaluationApiClient;
using LightOps.NeuralLens.Component.ObservabilityApiClient;
using LightOps.NeuralLens.Component.OrganizationApiClient;
using LightOps.NeuralLens.Component.ProjectApiClient;
using LightOps.NeuralLens.Component.ServiceDefaults;
using LightOps.NeuralLens.Frontend.Management.Components;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.Services
    .AddOrganizationApiClient("https+http://organization-api")
    .AddProjectApiClient("https+http://project-api")
    .AddObservabilityApiClient("https+http://observability-api")
    .AddEvaluationApiClient("https+http://evaluation-api");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddOutputCache();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();