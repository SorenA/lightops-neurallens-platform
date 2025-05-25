using LightOps.NeuralLens.OrganizationApi.Domain.Constants;
using LightOps.NeuralLens.OrganizationApi.Domain.Models;
using MongoDB.Driver;

namespace LightOps.NeuralLens.OrganizationApi.Domain.Repositories;

public class MongoOrganizationRepository(IMongoDatabase mongoDatabase) : IOrganizationRepository
{
    public Task<List<Organization>> GetAll()
    {
        return mongoDatabase
            .GetCollection<Organization>(MongoConstants.OrganizationCollection)
            .Find(FilterDefinition<Organization>.Empty)
            .ToListAsync();
    }

    public Task<Organization?> GetById(string id)
    {
        return mongoDatabase
            .GetCollection<Organization?>(MongoConstants.OrganizationCollection)
            .Find(o => o!.Id == id)
            .FirstOrDefaultAsync();
    }

    public Task<bool> NameExists(string name, string? exceptId = null)
    {
        return mongoDatabase
            .GetCollection<Organization>(MongoConstants.OrganizationCollection)
            .Find(o => o.Name == name && o.Id != exceptId)
            .AnyAsync();
    }

    public Task<Organization> Create(Organization organization)
    {
        return mongoDatabase
            .GetCollection<Organization>(MongoConstants.OrganizationCollection)
            .InsertOneAsync(organization)
            .ContinueWith(_ => organization);
    }

    public Task<Organization> Update(string id, Organization organization)
    {
        return mongoDatabase
            .GetCollection<Organization>(MongoConstants.OrganizationCollection)
            .FindOneAndReplaceAsync(o => o.Id == organization.Id, organization)
            .ContinueWith(_ => organization);
    }
}