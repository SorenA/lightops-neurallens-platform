using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace LightOps.NeuralLens.Component.ServiceDefaults.OpenApi;

public class SecuritySchemeDocumentTransformer : IOpenApiDocumentTransformer
{
    private string? _authIssuer;
    private readonly Dictionary<string, string> _authScopes = new();

    public SecuritySchemeDocumentTransformer WithAuthIssuer(string authIssuer)
    {
        _authIssuer = authIssuer;
        return this;
    }

    public SecuritySchemeDocumentTransformer AddScope(string scope, string? description = null)
    {
        _authScopes.Add(scope, description ?? string.Empty);
        return this;
    }

    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
        {
            ["Bearer"] = new()
            {
                Type = SecuritySchemeType.Http,
                Name = "Authorization",
                Scheme = "Bearer",
                In = ParameterLocation.Header,
                BearerFormat = "JWT",
            },
            ["OAuth2"] = new()
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{_authIssuer}/authorize", UriKind.RelativeOrAbsolute),
                        TokenUrl = new Uri($"{_authIssuer}/token", UriKind.RelativeOrAbsolute),
                        Scopes = _authScopes,
                        Extensions = new Dictionary<string, IOpenApiExtension>()
                        {
                            ["x-usePkce"] = new OpenApiString("SHA-256"),
                        },
                    },
                    ClientCredentials = new OpenApiOAuthFlow
                    {
                        TokenUrl = new Uri($"{_authIssuer}/token", UriKind.RelativeOrAbsolute),
                        Scopes = _authScopes,
                        Extensions = new Dictionary<string, IOpenApiExtension>()
                        {
                            ["x-usePkce"] = new OpenApiString("SHA-256"),
                        },
                    },
                },
            },
            ["OIDC"] = new()
            {
                Type = SecuritySchemeType.OpenIdConnect,
                OpenIdConnectUrl = new Uri($"{_authIssuer}/.well-known/openid-configuration", UriKind.RelativeOrAbsolute),
                Extensions = new Dictionary<string, IOpenApiExtension>()
                {
                    ["x-usePkce"] = new OpenApiString("SHA-256"),
                },
            },
        };

        return Task.CompletedTask;
    }
}