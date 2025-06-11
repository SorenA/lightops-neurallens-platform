using LightOps.NeuralLens.AuthApi.Domain.Constants;
using LightOps.NeuralLens.AuthApi.Domain.Models;
using MongoDB.Driver;

namespace LightOps.NeuralLens.AuthApi.Domain.Repositories;

public class MongoApplicationUserRepository(IMongoDatabase mongoDatabase) : IApplicationUserRepository
{
    private IMongoCollection<ApplicationUser> Collection => mongoDatabase
        .GetCollection<ApplicationUser>(MongoConstants.ApplicationUserCollection);

    public Task<List<ApplicationUser>> GetAll()
    {
        return Collection
            .Find(x => true)
            .ToListAsync();
    }

    public Task<ApplicationUser?> GetById(string id)
    {
        return Collection
            .Find(m => m.Id == id)
            .FirstOrDefaultAsync()!;
    }

    public Task<ApplicationUser?> GetByExternalId(string provider, string id)
    {
        return Collection
            .Find(m => m.ExternalIds.ContainsKey(provider) && m.ExternalIds[provider] == id)
            .FirstOrDefaultAsync()!;
    }

    public Task<ApplicationUser> Create(ApplicationUser user)
    {
        return Collection
            .InsertOneAsync(user)
            .ContinueWith(_ => user);
    }

    public Task<ApplicationUser> Update(string id, ApplicationUser user)
    {
        return Collection
            .FindOneAndReplaceAsync(
                m => m.Id == id,
                user)
            .ContinueWith(_ => user);
    }
}