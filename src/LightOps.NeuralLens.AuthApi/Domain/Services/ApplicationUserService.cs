using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using LightOps.NeuralLens.AuthApi.Domain.Models;
using LightOps.NeuralLens.AuthApi.Domain.Repositories;
using LightOps.NeuralLens.AuthApi.Requests;
using LightOps.NeuralLens.Component.ServiceDefaults;

namespace LightOps.NeuralLens.AuthApi.Domain.Services;

public class ApplicationUserService(
    IApplicationUserRepository applicationUserRepository)
{
    public async Task<List<ApplicationUser>> GetAll()
    {
        return await applicationUserRepository.GetAll();
    }

    public async Task<ApplicationUser?> GetById(string id)
    {
        return await applicationUserRepository.GetById(id);
    }

    public async Task<ApplicationUser?> GetByExternalId(string provider, string id)
    {
        return await applicationUserRepository.GetByExternalId(provider, id);
    }

    public async Task<ApplicationUser> GetByExternalIdOrCreate(string provider, string id)
    {
        return (await GetByExternalId(provider, id))
               ?? await Create(new CreateApplicationUserRequest());
    }

    public async Task<ApplicationUser> Create(CreateApplicationUserRequest request)
    {
        // Map request to domain model
        var entity = new ApplicationUser(
            Guid.NewGuid().ToString(),
            DateTime.UtcNow,
            DateTime.UtcNow);

        return await applicationUserRepository.Create(entity);
    }

    public async Task<ApplicationUser?> Update(string id, UpdateApplicationUserRequest request)
    {
        var entity = await applicationUserRepository.GetById(id);
        if (entity == null)
        {
            return null;
        }

        // Map request to domain model
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            entity.Name = request.Name;
        }

        if (!string.IsNullOrWhiteSpace(request.PictureUrl))
        {
            entity.PictureUrl = request.PictureUrl;
        }

        if (!string.IsNullOrWhiteSpace(request.ExternalProvider) && !string.IsNullOrWhiteSpace(request.ExternalId))
        {
            entity.ExternalIds[request.ExternalProvider] = request.ExternalId;
        }

        entity.UpdatedAt = DateTime.UtcNow;

        return await applicationUserRepository.Update(id, entity);
    }

    public ClaimsIdentity GetClaimsIdentity(ApplicationUser user, string authenticationType = "ExternalLogin")
    {
        var identity = new ClaimsIdentity(
            authenticationType: authenticationType,
            nameType: ClaimTypes.Name,
            roleType: ClaimTypes.Role);

        identity.AddClaims([
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(AuthClaims.Name, user.Name ?? string.Empty),
            new Claim(AuthClaims.Picture, user.PictureUrl ?? string.Empty),
            new Claim(AuthClaims.UpdatedAt, user.UpdatedAt.ToString("O")),
        ]);

        return identity;
    }
}