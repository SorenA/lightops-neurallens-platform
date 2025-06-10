using System.Security.Claims;
using LightOps.NeuralLens.Component.EvaluationApiConnector;
using LightOps.NeuralLens.Component.ObservabilityApiConnector;
using LightOps.NeuralLens.Component.OrganizationApiConnector;
using LightOps.NeuralLens.Component.ServiceDefaults;
using LightOps.NeuralLens.Component.WorkspaceApiConnector;
using LightOps.NeuralLens.ManagementFrontend.Components;
using LightOps.NeuralLens.ManagementFrontend.Domain.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.Services
    .AddOrganizationApiConnector("https+http://organization-api")
    .AddWorkspaceApiConnector("https+http://workspace-api")
    .AddObservabilityApiConnector("https+http://observability-api")
    .AddEvaluationApiConnector("https+http://evaluation-api");

// Add states
builder.Services.AddTransient<OrganizationService>();
builder.Services.AddTransient<WorkspaceService>();

// Add Auth
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddOpenIdConnect(options =>
    {
        options.Authority = builder.Configuration.GetValue<string>("Services:auth-api:Https:0")!;
        options.ClientId = builder.Configuration.GetValue<string>("Services:auth-api:ClientId")!;
        options.Scope.Add(AuthScopes.Organizations.Read);
        options.Scope.Add(AuthScopes.Organizations.Write);
        options.Scope.Add(AuthScopes.Workspaces.Read);
        options.Scope.Add(AuthScopes.Workspaces.Write);

        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.ResponseType = OpenIdConnectResponseType.Code;

        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;

        options.MapInboundClaims = false;
        options.TokenValidationParameters.NameClaimType = ClaimTypes.Name;
        options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
    });

// Add services to the container.
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
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