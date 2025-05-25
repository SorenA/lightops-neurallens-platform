using LightOps.NeuralLens.OrganizationApi.Domain.Constants;
using LightOps.NeuralLens.OrganizationApi.Domain.Exceptions;
using LightOps.NeuralLens.OrganizationApi.Domain.Models;
using MongoDB.Driver;

namespace LightOps.NeuralLens.OrganizationApi.Domain.Repositories;

public class MongoOrganizationRepository(IMongoDatabase mongoDatabase) : IOrganizationRepository
{
    private IMongoCollection<Organization> Collection => mongoDatabase
        .GetCollection<Organization>(MongoConstants.OrganizationCollection);

    public Task<List<Organization>> GetAll()
    {
        return Collection
            .Find(m =>
                !m.IsDeleted)
            .ToListAsync();
    }

    public Task<Organization?> GetById(string id)
    {
        return Collection
            .Find(m =>
                !m.IsDeleted
                && m.Id == id)
            .FirstOrDefaultAsync()!;
    }

    public Task<bool> NameExists(string name, string? exceptId = null)
    {
        return Collection
            .Find(m =>
                !m.IsDeleted
                && m.Name == name
                && m.Id != exceptId)
            .AnyAsync();
    }

    public Task<Organization> Create(Organization organization)
    {
        return Collection
            .InsertOneAsync(organization)
            .ContinueWith(_ => organization);
    }

    public Task<Organization> Update(string id, Organization organization)
    {
        return Collection
            .FindOneAndReplaceAsync(
                m =>
                    !m.IsDeleted
                    && m.Id == id,
                organization)
            .ContinueWith(_ => organization);
    }

    public Task<Organization> Delete(string id)
    {
        return Collection
            .FindOneAndDeleteAsync(m =>
                m.Id == id)
            .ContinueWith(task => task.Result ?? throw new OrganizationNotFoundException(id));
    }
}