using LightOps.NeuralLens.WorkspaceApi.Domain.Constants;
using LightOps.NeuralLens.WorkspaceApi.Domain.Exceptions;
using LightOps.NeuralLens.WorkspaceApi.Domain.Models;
using MongoDB.Driver;

namespace LightOps.NeuralLens.WorkspaceApi.Domain.Repositories;

public class MongoWorkspaceRepository(IMongoDatabase mongoDatabase) : IWorkspaceRepository
{
    private IMongoCollection<Workspace> Collection => mongoDatabase
        .GetCollection<Workspace>(MongoConstants.WorkspaceCollection);

    public Task<List<Workspace>> GetAll(string organizationId)
    {
        return Collection
            .Find(m =>
                !m.IsDeleted
                && m.OrganizationId == organizationId)
            .ToListAsync();
    }

    public Task<Workspace?> GetById(string organizationId, string id)
    {
        return Collection
            .Find(m =>
                !m.IsDeleted
                && m.OrganizationId == organizationId
                && m.Id == id)
            .FirstOrDefaultAsync()!;
    }

    public Task<bool> NameExists(string organizationId, string name, string? exceptId = null)
    {
        return Collection
            .Find(m =>
                !m.IsDeleted
                && m.OrganizationId == organizationId
                && m.Name == name
                && m.Id != exceptId)
            .AnyAsync();
    }

    public Task<Workspace> Create(string organizationId, Workspace workspace)
    {
        return Collection
            .InsertOneAsync(workspace)
            .ContinueWith(_ => workspace);
    }

    public Task<Workspace> Update(string organizationId, string id, Workspace workspace)
    {
        return Collection
            .FindOneAndReplaceAsync(
                m =>
                    !m.IsDeleted
                    && m.OrganizationId == organizationId
                    && m.Id == id,
                workspace)
            .ContinueWith(_ => workspace);
    }

    public Task<Workspace> Delete(string organizationId, string id)
    {
        return Collection
            .FindOneAndDeleteAsync(m =>
                m.OrganizationId == organizationId
                && m.Id == id)
            .ContinueWith(task => task.Result ?? throw new WorkspaceNotFoundException(id));
    }
}