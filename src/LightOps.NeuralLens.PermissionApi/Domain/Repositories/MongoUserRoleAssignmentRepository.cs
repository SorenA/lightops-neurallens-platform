using LightOps.NeuralLens.PermissionApi.Domain.Constants;
using LightOps.NeuralLens.PermissionApi.Domain.Models;
using MongoDB.Driver;

namespace LightOps.NeuralLens.PermissionApi.Domain.Repositories;

public class MongoUserRoleAssignmentRepository(IMongoDatabase mongoDatabase) : IUserRoleAssignmentRepository
{
    private IMongoCollection<UserRoleAssignment> Collection => mongoDatabase
        .GetCollection<UserRoleAssignment>(MongoConstants.UserRoleAssignmentCollection);

    public Task<List<UserRoleAssignment>> GetByUserId(string userId)
    {
        return Collection
            .Find(m =>
                m.UserId == userId)
            .ToListAsync();
    }

    public Task<List<UserRoleAssignment>> GetByUserAssignedScope(string userId, params string[] assignedScopes)
    {
        return Collection
            .Find(m =>
                m.UserId == userId
                && assignedScopes.Contains(m.AssignedScope))
            .ToListAsync();
    }

    public Task<List<UserRoleAssignment>> GetByAssignedScope(params string[] assignedScopes)
    {
        return Collection
            .Find(m =>
            assignedScopes.Contains(m.AssignedScope))
            .ToListAsync();
    }

    public Task<UserRoleAssignment?> GetById(string id)
    {
        return Collection
            .Find(m => m.Id == id)
            .FirstOrDefaultAsync()!;
    }

    public Task<UserRoleAssignment> Create(UserRoleAssignment userRoleAssignment)
    {
        return Collection
            .InsertOneAsync(userRoleAssignment)
            .ContinueWith(_ => userRoleAssignment);
    }

    public Task<UserRoleAssignment?> Delete(string id)
    {
        return Collection
            .FindOneAndDeleteAsync(m =>
                m.Id == id)
            .ContinueWith(task => task.Result)!;
    }
}