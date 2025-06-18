using FluentValidation;
using LightOps.Mapping.Api.Mappers;
using LightOps.NeuralLens.Component.ServiceDefaults;
using LightOps.NeuralLens.Component.ServiceDefaults.OpenApi;
using LightOps.NeuralLens.WorkspaceApi.Domain.Mappers;
using LightOps.NeuralLens.WorkspaceApi.Domain.Models;
using LightOps.NeuralLens.WorkspaceApi.Domain.Repositories;
using LightOps.NeuralLens.WorkspaceApi.Domain.RequestValidators;
using LightOps.NeuralLens.WorkspaceApi.Domain.Services;
using LightOps.NeuralLens.WorkspaceApi.Models;
using LightOps.NeuralLens.WorkspaceApi.Requests;
using OpenIddict.Abstractions;
using OpenIddict.Client;
using OpenIddict.Validation.AspNetCore;

namespace LightOps.NeuralLens.WorkspaceApi.Extensions
{
    public static class StartupExtensions
    {
        /// <summary>
        /// Add runtime services to the application builder.
        /// </summary>
        /// <param name="builder">The application builder to use for registration.</param>
        public static void AddRuntimeServices(this IHostApplicationBuilder builder)
        {
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
        }

        /// <summary>
        /// Add runtime authentication to the application builder.
        /// </summary>
        /// <param name="builder">The application builder to use for registration.</param>
        public static void AddRuntimeAuth(this IHostApplicationBuilder builder)
        {
            var authIssuer = builder.Configuration.GetValue<string>("Services:auth-api:Https:0")!;
            var authClientId = builder.Configuration.GetValue<string>("Services:auth-api:ClientId")!;
            var authClientSecret = builder.Configuration.GetValue<string>("Services:auth-api:ClientSecret")!;

            builder.Services.AddOpenIddict()
                .AddClient(options =>
                {
                    // Configure for client credentials flow
                    options.AllowClientCredentialsFlow();
                    options.DisableTokenStorage();
                    options.UseSystemNetHttp().SetProductInformation(typeof(StartupExtensions).Assembly);
                    options.AddRegistration(new OpenIddictClientRegistration
                    {
                        Issuer = new Uri(authIssuer, UriKind.Absolute),
                        ClientId = authClientId,
                        ClientSecret = authClientSecret,
                    });
                })
                .AddValidation(options =>
                {
                    options.SetIssuer(authIssuer);
                    options.AddAudiences(AuthAudiences.WorkspaceApi);
                    options.UseIntrospection()
                        .SetClientId(authClientId)
                        .SetClientSecret(authClientSecret);

                    options.UseSystemNetHttp();
                    options.UseAspNetCore();
                });

            builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthScopes.Workspaces.Read, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireAssertion(x => x.User.HasScope(AuthScopes.Workspaces.Read));
                });
                options.AddPolicy(AuthScopes.Workspaces.Write, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireAssertion(x => x.User.HasScope(AuthScopes.Workspaces.Write));
                });
            });
        }

        /// <summary>
        /// Use runtime authentication in the application.
        /// </summary>
        /// <param name="app">The application to use auth in.</param>
        public static void UseRuntimeAuth(this WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        /// <summary>
        /// Add OpenAPI specification generation to the application builder.
        /// </summary>
        /// <param name="builder">The application builder to use for registration.</param>
        public static void AddRuntimeOpenApiSpecification(this IHostApplicationBuilder builder)
        {
            builder.Services.AddOpenApi("v1", options =>
            {
                options.AddDocumentTransformer(new RuntimeDocumentTransformer()
                    .WithInfo(
                        "Workspace API",
                        "A Web API for workspaces as part of the LightOps NeuralLens Platform."));
                options.AddDocumentTransformer(new SecuritySchemeDocumentTransformer()
                    .WithAuthIssuer(builder.Configuration.GetValue<string>("Services:auth-api:Https:0")!)
                    .AddScope(AuthScopes.Workspaces.Read, "Read workspaces")
                    .AddScope(AuthScopes.Workspaces.Write, "Write workspaces"));
                options.AddOperationTransformer<SecuritySchemeOperationTransformer>();
            });
        }

        /// <summary>
        /// Serve OpenAPI specification in the application.
        /// </summary>
        /// <param name="app">The application to serve from.</param>
        public static void UseRuntimeOpenApiSpecification(this WebApplication app)
        {
            app.MapOpenApi();
        }

        /// <summary>
        /// Add API versioning to the application builder.
        /// </summary>
        /// <param name="builder">The application builder to use for registration.</param>
        public static void AddRuntimeApiVersioning(this IHostApplicationBuilder builder)
        {
            builder.Services.AddApiVersioning(
                    options =>
                    {
                        options.ReportApiVersions = true;
                    })
                .AddApiExplorer(
                    options =>
                    {
                        options.GroupNameFormat = "'v'VVV";
                        options.SubstituteApiVersionInUrl = true;
                    });
        }
    }
}
