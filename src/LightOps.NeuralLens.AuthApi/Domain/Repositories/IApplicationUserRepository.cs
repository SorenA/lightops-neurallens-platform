using LightOps.NeuralLens.AuthApi.Domain.Models;

namespace LightOps.NeuralLens.AuthApi.Domain.Repositories;

public interface IApplicationUserRepository
{
    Task<List<ApplicationUser>> GetAll();
    Task<ApplicationUser?> GetById(string id);
    Task<ApplicationUser?> GetByExternalId(string provider, string id);
    Task<ApplicationUser> Create(ApplicationUser user);
    Task<ApplicationUser> Update(string id, ApplicationUser user);
}