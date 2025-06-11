using LightOps.NeuralLens.Component.ServiceDefaults;
using OpenIddict.Abstractions;
using OpenIddict.Server;

namespace LightOps.NeuralLens.AuthApi.Domain.EventHandlers;

public class UserInfoRequestContextEventHandler : IOpenIddictServerHandler<OpenIddictServerEvents.HandleUserInfoRequestContext>
{
    public ValueTask HandleAsync(OpenIddictServerEvents.HandleUserInfoRequestContext context)
    {
        if (context.Principal.HasScope(AuthScopes.Profile))
        {
            context.Claims[AuthClaims.Name] = context.Principal.GetClaim(AuthClaims.Name);
            context.Claims[AuthClaims.Picture] = context.Principal.GetClaim(AuthClaims.Picture);
            context.Claims[AuthClaims.UpdatedAt] = context.Principal.GetClaim(AuthClaims.UpdatedAt);
        }

        return default;
    }
}