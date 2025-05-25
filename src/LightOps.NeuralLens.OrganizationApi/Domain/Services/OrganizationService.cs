using LightOps.NeuralLens.OrganizationApi.Domain.Exceptions;
using LightOps.NeuralLens.OrganizationApi.Domain.Models;
using LightOps.NeuralLens.OrganizationApi.Domain.Repositories;
using LightOps.NeuralLens.OrganizationApi.Requests;

namespace LightOps.NeuralLens.OrganizationApi.Domain.Services;

public class OrganizationService(
    IOrganizationRepository organizationRepository)
{
    public async Task<List<Organization>> GetAll()
    {
        return await organizationRepository.GetAll();
    }

    public async Task<Organization> GetById(string id)
    {
        var entity = await organizationRepository.GetById(id);
        if (entity == null)
        {
            throw new OrganizationNotFoundException(id);
        }

        return entity;
    }

    public async Task<Organization> Create(CreateOrganizationRequest request)
    {
        // Map request to domain model
        var entity = new Organization(
            Guid.NewGuid().ToString(),
            request.Name,
            DateTime.UtcNow,
            DateTime.UtcNow)
        {
            Description = request.Description ?? string.Empty,
        };

        return await organizationRepository.Create(entity);
    }

    public async Task<Organization> Update(string id, UpdateOrganizationRequest request)
    {
        var entity = await organizationRepository.GetById(id);
        if (entity == null)
        {
            throw new OrganizationNotFoundException(id);
        }

        // Map request to domain model
        entity.Name = request.Name;
        entity.Description = request.Description ?? string.Empty;
        entity.UpdatedAt = DateTime.UtcNow;

        return await organizationRepository.Update(id, entity);
    }

    public async Task<Organization> Delete(string id)
    {
        var entity = await organizationRepository.GetById(id);
        if (entity == null)
        {
            throw new OrganizationNotFoundException(id);
        }

        // Soft-delete
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;

        return await organizationRepository.Update(id, entity);
    }
}