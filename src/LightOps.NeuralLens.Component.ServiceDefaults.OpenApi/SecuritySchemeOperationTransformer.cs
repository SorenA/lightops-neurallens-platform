using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace LightOps.NeuralLens.Component.ServiceDefaults.OpenApi;

public class SecuritySchemeOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        // Get policy applied to endpoint
        var requiredPolicies = GetMethodInfo(context.Description)?
            .GetCustomAttributes(true)
            .OfType<AuthorizeAttribute>()
            .Select(attribute => attribute.Policy)
            .Distinct()
            .ToList();
        if (requiredPolicies == null || requiredPolicies.Count == 0)
        {
            // No policies required, skip adding security requirements
            return Task.CompletedTask;
        }

        // Add authorization related responses
        operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
        operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

        // Add security requirements to the operation
        var bearerScheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
        };
        var oidcScheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "OIDC" },
        };
        var oauthScheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "OAuth2" },
        };
        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new()
            {
                [oidcScheme] = [.. requiredPolicies],
                [oauthScheme] = [.. requiredPolicies],
                [bearerScheme] = [.. requiredPolicies],
            },
        };

        return Task.CompletedTask;
    }

    private static MethodInfo? GetMethodInfo(ApiDescription apiDescription)
    {
        if (apiDescription.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        {
            return controllerActionDescriptor.MethodInfo;
        }

        return apiDescription.ActionDescriptor?
            .EndpointMetadata?
            .OfType<MethodInfo>()
            .FirstOrDefault();
    }
}