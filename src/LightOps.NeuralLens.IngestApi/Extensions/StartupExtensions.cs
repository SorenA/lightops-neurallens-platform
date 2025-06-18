using LightOps.Mapping.Api.Mappers;
using LightOps.NeuralLens.Component.ServiceDefaults;
using LightOps.NeuralLens.Component.ServiceDefaults.OpenApi;
using LightOps.NeuralLens.IngestApi.Domain.Mappers;
using LightOps.NeuralLens.IngestApi.Domain.MappingModels;
using OpenIddict.Client;
using OpenIddict.Validation.AspNetCore;
using OpenTelemetry.Proto.Trace.V1;

namespace LightOps.NeuralLens.IngestApi.Extensions
{
    public static class StartupExtensions
    {
        /// <summary>
        /// Add runtime services to the application builder.
        /// </summary>
        /// <param name="builder">The application builder to use for registration.</param>
        public static void AddRuntimeServices(this IHostApplicationBuilder builder)
        {
            // Add mappers
            builder.Services.AddTransient<IMapper<ResourceSpans, ObservabilityTraceMappingResult?>, OpenTelemetryTraceMapper>();
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
                    options.AddAudiences(AuthAudiences.IngestApi);
                    options.UseIntrospection()
                        .SetClientId(authClientId)
                        .SetClientSecret(authClientSecret);

                    options.UseSystemNetHttp();
                    options.UseAspNetCore();
                });

            builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
            builder.Services.AddAuthorization(options =>
            {
                // No policies
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
                        "Ingest API",
                        "A Web API for ingest as part of the LightOps NeuralLens Platform."));
                options.AddDocumentTransformer(new SecuritySchemeDocumentTransformer()
                    .WithAuthIssuer(builder.Configuration.GetValue<string>("Services:auth-api:Https:0")!));
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
