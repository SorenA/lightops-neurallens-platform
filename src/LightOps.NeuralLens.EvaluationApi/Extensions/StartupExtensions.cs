using LightOps.NeuralLens.Component.ServiceDefaults;
using LightOps.NeuralLens.Component.ServiceDefaults.OpenApi;
using OpenIddict.Client;
using OpenIddict.Validation.AspNetCore;

namespace LightOps.NeuralLens.EvaluationApi.Extensions
{
    public static class StartupExtensions
    {
        /// <summary>
        /// Add runtime services to the application builder.
        /// </summary>
        /// <param name="builder">The application builder to use for registration.</param>
        public static void AddRuntimeServices(this IHostApplicationBuilder builder)
        {
            // None
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
                    options.AddAudiences(AuthAudiences.EvaluationApi);
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
                        "Evaluation API",
                        "A Web API for evaluation as part of the LightOps NeuralLens Platform."));
                options.AddDocumentTransformer(new SecuritySchemeDocumentTransformer()
                    .WithAuthIssuer(builder.Configuration.GetValue<string>("Services:api-gateway:Https:0")!));
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
