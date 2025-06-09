using LightOps.NeuralLens.OrganizationApi.Domain.Models;

namespace LightOps.NeuralLens.OrganizationApi.Domain.Repositories;

public interface IOrganizationRepository
{
    Task<List<Organization>> GetAll();
    Task<Organization?> GetById(string id);
    Task<bool> NameExists(string name, string? exceptId = null);
    Task<Organization> Create(Organization organization);
    Task<Organization?> Update(string id, Organization organization);
    Task<Organization?> Delete(string id);
}