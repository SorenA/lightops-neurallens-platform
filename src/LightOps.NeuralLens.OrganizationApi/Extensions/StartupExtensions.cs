using FluentValidation;
using LightOps.Mapping.Api.Mappers;
using LightOps.NeuralLens.Component.ServiceDefaults;
using LightOps.NeuralLens.Component.ServiceDefaults.OpenApi;
using LightOps.NeuralLens.OrganizationApi.Domain.Mappers;
using LightOps.NeuralLens.OrganizationApi.Domain.Models;
using LightOps.NeuralLens.OrganizationApi.Domain.Repositories;
using LightOps.NeuralLens.OrganizationApi.Domain.RequestValidators;
using LightOps.NeuralLens.OrganizationApi.Domain.Services;
using LightOps.NeuralLens.OrganizationApi.Models;
using LightOps.NeuralLens.OrganizationApi.Requests;
using OpenIddict.Abstractions;
using OpenIddict.Client;
using OpenIddict.Validation.AspNetCore;

namespace LightOps.NeuralLens.OrganizationApi.Extensions
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
            builder.Services.AddTransient<IOrganizationRepository, MongoOrganizationRepository>();

            // Add services
            builder.Services.AddTransient<OrganizationService>();

            // Add mappers
            builder.Services.AddTransient<IMapper<Organization, OrganizationViewModel>, OrganizationViewModelMapper>();

            // Add validators
            builder.Services.AddScoped<IValidator<CreateOrganizationRequest>, CreateOrganizationRequestValidator>();
            builder.Services.AddScoped<IValidator<UpdateOrganizationRequest>, UpdateOrganizationRequestValidator>();
        }

        /// <summary>
        /// Add runtime authentication to the application builder.
        /// </summary>
        /// <param name="builder">The application builder to use for registration.</param>
        public static void AddRuntimeAuth(this IHostApplicationBuilder builder)
        {
            var authIssuer = builder.Configuration.GetValue<string>("Services:api-gateway:Https:0")!;
            var authClientId = builder.Configuration.GetValue<string>("AuthManagedIdentity:ClientId")!;
            var authClientSecret = builder.Configuration.GetValue<string>("AuthManagedIdentity:ClientSecret")!;

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
                    options.AddAudiences(AuthAudiences.OrganizationApi);
                    options.UseIntrospection()
                        .SetClientId(authClientId)
                        .SetClientSecret(authClientSecret);

                    options.UseSystemNetHttp();
                    options.UseAspNetCore();
                });

            builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthScopes.Organizations.Read, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireAssertion(x => x.User.HasScope(AuthScopes.Organizations.Read));
                });
                options.AddPolicy(AuthScopes.Organizations.Write, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireAssertion(x => x.User.HasScope(AuthScopes.Organizations.Write));
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
                        "Organization API",
                        "A Web API for organizations as part of the LightOps NeuralLens Platform."));
                options.AddDocumentTransformer(new SecuritySchemeDocumentTransformer()
                    .WithAuthIssuer(builder.Configuration.GetValue<string>("Services:api-gateway:Https:0")!)
                    .AddScope(AuthScopes.Organizations.Read, "Read organizations")
                    .AddScope(AuthScopes.Organizations.Write, "Write organizations"));
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
