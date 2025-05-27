using LightOps.NeuralLens.Component.EvaluationApiConnector;
using LightOps.NeuralLens.Component.ObservabilityApiConnector;
using LightOps.NeuralLens.Component.OrganizationApiConnector;
using LightOps.NeuralLens.Component.ServiceDefaults;
using LightOps.NeuralLens.Component.WorkspaceApiConnector;
using LightOps.NeuralLens.Frontend.Management.Components;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.Services
    .AddOrganizationApiConnector("https+http://organization-api")
    .AddWorkspaceApiConnector("https+http://workspace-api")
    .AddObservabilityApiConnector("https+http://observability-api")
    .AddEvaluationApiConnector("https+http://evaluation-api");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();
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