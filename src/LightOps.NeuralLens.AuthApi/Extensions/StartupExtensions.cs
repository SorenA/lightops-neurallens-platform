using LightOps.NeuralLens.AuthApi.Domain.EventHandlers;
using LightOps.NeuralLens.AuthApi.Domain.Repositories;
using LightOps.NeuralLens.AuthApi.Domain.Services;
using LightOps.NeuralLens.Component.ServiceDefaults.OpenApi;
using Microsoft.AspNetCore.Authentication.Cookies;
using OpenIddict.Server;

namespace LightOps.NeuralLens.AuthApi.Extensions
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
            builder.Services.AddTransient<IApplicationUserRepository, MongoApplicationUserRepository>();

            // Add services
            builder.Services.AddTransient<ApplicationUserService>();

            // Add event handlers
            builder.Services.AddScoped<IOpenIddictServerHandler<OpenIddictServerEvents.HandleUserInfoRequestContext>, UserInfoRequestContextEventHandler>();
        }

        /// <summary>
        /// Add runtime authentication to the application builder.
        /// </summary>
        /// <param name="builder">The application builder to use for registration.</param>
        public static void AddRuntimeAuth(this IHostApplicationBuilder builder)
        {
            var apiGatewayBase = builder.Configuration.GetValue<string>("Services:api-gateway:Https:0")!;

            builder.Services.AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseMongoDb();
                })
                .AddClient(options =>
                {
                    // Enable authorization code and refresh token flows.
                    options.AllowAuthorizationCodeFlow();

                    if (builder.Environment.IsDevelopment())
                    {
                        // Register the signing and encryption credentials.
                        options.AddDevelopmentEncryptionCertificate()
                            .AddDevelopmentSigningCertificate();
                    }

                    options.UseAspNetCore()
                        .EnableRedirectionEndpointPassthrough();

                    options.UseWebProviders()
                        .AddGitHub(o =>
                        {
                            o.SetClientId(builder.Configuration.GetValue<string>("AuthProviders:GitHub:ClientId")!)
                                .SetClientSecret(
                                    builder.Configuration.GetValue<string>("AuthProviders:GitHub:ClientSecret")!)
                                .SetRedirectUri($"{apiGatewayBase}/v1/auth/callback/login/github");
                        });
                })
                .AddServer(options =>
                {
                    options
                        .UseReferenceRefreshTokens();

                    // Enable the authorization, introspection and token endpoints, map first through
                    // API gateway for OIDC discovery and then enable the local auth API itself
                    options.SetIssuer($"{apiGatewayBase}")
                        .SetJsonWebKeySetEndpointUris($"{apiGatewayBase}/v1/auth/jwks", "/v1/auth/jwks")
                        .SetAuthorizationEndpointUris($"{apiGatewayBase}/v1/auth/authorize", "/v1/auth/authorize")
                        .SetIntrospectionEndpointUris($"{apiGatewayBase}/v1/auth/callback/login/github", "/v1/auth/callback/login/github")
                        .SetTokenEndpointUris($"{apiGatewayBase}/v1/auth/token", "/v1/auth/token")
                        .SetUserInfoEndpointUris($"{apiGatewayBase}/v1/auth/userinfo", "/v1/auth/userinfo")
                        .SetEndSessionEndpointUris($"{apiGatewayBase}/v1/auth/endsession", "/v1/auth/endsession");

                    // Enable authorization code and refresh token flows
                    options.AllowAuthorizationCodeFlow()
                        .AllowRefreshTokenFlow()
                        .AllowClientCredentialsFlow();

                    options.AddEventHandler<OpenIddictServerEvents.HandleUserInfoRequestContext>(b => b.UseScopedHandler<UserInfoRequestContextEventHandler>());

                    if (builder.Environment.IsDevelopment())
                    {
                        // Register the signing and encryption credentials
                        options.AddDevelopmentEncryptionCertificate()
                            .AddDevelopmentSigningCertificate();
                    }

                    options.UseAspNetCore()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableEndSessionEndpointPassthrough();
                })
                .AddValidation(options =>
                {
                    options.UseLocalServer();
                    options.UseAspNetCore();
                });

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();
            builder.Services.AddAuthorization();
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
                        "Auth API",
                        "A Web API for auth as part of the LightOps NeuralLens Platform."));
                options.AddDocumentTransformer(new SecuritySchemeDocumentTransformer()
                    .WithAuthIssuer(string.Empty));
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
