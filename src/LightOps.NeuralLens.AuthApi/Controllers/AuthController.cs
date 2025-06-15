using System.Collections.Immutable;
using System.Security.Claims;
using LightOps.NeuralLens.AuthApi.Domain.Services;
using LightOps.NeuralLens.AuthApi.Requests;
using LightOps.NeuralLens.Component.ServiceDefaults;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Client.WebIntegration;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace LightOps.NeuralLens.AuthApi.Controllers;

[ApiController]
[Route("")]
public class AuthController(
    ApplicationUserService applicationUserService,
    IOpenIddictScopeManager scopeManager)
    : ControllerBase
{
    [HttpGet("authorize", Name = "GetAuthorize")]
    [HttpPost("authorize", Name = "PostAuthorize")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IResult> Authorize()
    {
        var request = HttpContext.GetOpenIddictServerRequest();

        // Check if the user is authenticated via the local authentication scheme
        var authResult = await HttpContext.AuthenticateAsync();
        var principal = authResult?.Principal;
        if (principal is not { Identity.IsAuthenticated: true })
        {
            // User not authenticated, redirect to GitHub
            var properties = new AuthenticationProperties
            {
                RedirectUri = HttpContext.Request.GetEncodedUrl(),
            };
            return Results.Challenge(properties, [OpenIddictClientWebIntegrationConstants.Providers.GitHub]);
        }

        // Get application user
        var applicationUserId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var applicationUser = await applicationUserService.GetById(applicationUserId);
        if (applicationUser == null)
        {
            // User doesn't exist, redirect to GitHub
            var properties = new AuthenticationProperties
            {
                RedirectUri = HttpContext.Request.GetEncodedUrl(),
            };
            return Results.Challenge(properties, [OpenIddictClientWebIntegrationConstants.Providers.GitHub]);
        }

        // Create the claims-based identity that will be used by OpenIddict to generate tokens
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);
        identity.AddClaim(new Claim(Claims.Subject, applicationUser.Id));
        identity.SetScopes(request?.GetScopes());

        // Add audiences based on resources
        var resources = scopeManager
            .ListResourcesAsync(request?.GetScopes() ?? [])
            .ToBlockingEnumerable();
        identity.SetResources(resources);

        // Pass profile claims if requested
        if (request?.HasScope(AuthScopes.Profile) ?? false)
        {
            identity.AddClaim(new Claim(Claims.Name, applicationUser.Name ?? string.Empty)
                .SetDestinations(Destinations.AccessToken));
            identity.AddClaim(new Claim(Claims.PreferredUsername, applicationUser.Name ?? string.Empty)
                .SetDestinations(Destinations.AccessToken));
            identity.AddClaim(new Claim(Claims.Picture, applicationUser.PictureUrl ?? string.Empty)
                .SetDestinations(Destinations.AccessToken));
            identity.AddClaim(new Claim(Claims.UpdatedAt, applicationUser.UpdatedAt.ToString("O"))
                .SetDestinations(Destinations.AccessToken));
        }

        return Results.SignIn(new ClaimsPrincipal(identity), properties: null, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    [HttpGet("callback/login/github", Name = "GetCallbackLoginGitHub")]
    [HttpPost("callback/login/github", Name = "PostCallbackLoginGitHub")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> CallbackLoginGitHub()
    {
        // Resolve the claims extracted by OpenIddict from the userinfo response returned by GitHub
        var result = await HttpContext.AuthenticateAsync(OpenIddictClientWebIntegrationConstants.Providers.GitHub);

        // Get application user
        var externalId = result.Principal!.FindFirstValue("id")!;
        var applicationUser =
            await applicationUserService.GetByExternalIdOrCreate(
                OpenIddictClientWebIntegrationConstants.Providers.GitHub,
                externalId);

        // Update with claims from upstream
        await applicationUserService.Update(applicationUser.Id, new UpdateApplicationUserRequest(
            result.Principal!.FindFirstValue("name"),
            result.Principal!.FindFirstValue("avatar_url"),
            OpenIddictClientWebIntegrationConstants.Providers.GitHub,
            externalId));

        // Create claims identity
        var identity = applicationUserService.GetClaimsIdentity(applicationUser);

        // Sign in the user and redirect to the original URL
        var properties = new AuthenticationProperties
        {
            RedirectUri = result.Properties!.RedirectUri,
        };
        return Results.SignIn(new ClaimsPrincipal(identity), properties);
    }

    [HttpGet("endsession", Name = "GetEndSession")]
    [HttpPost("endsession", Name = "PostEndSession")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IResult EndSession()
    {
        var request = HttpContext.GetOpenIddictServerRequest();

        return Results.SignOut(
            authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
            properties: new AuthenticationProperties
            {
                RedirectUri = request?.PostLogoutRedirectUri,
            });
    }
}
