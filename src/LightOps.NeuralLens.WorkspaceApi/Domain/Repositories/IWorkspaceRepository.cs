using LightOps.NeuralLens.WorkspaceApi.Domain.Models;

namespace LightOps.NeuralLens.WorkspaceApi.Domain.Repositories;

public interface IWorkspaceRepository
{
    Task<List<Workspace>> GetAll(string organizationId);
    Task<Workspace?> GetById(string organizationId, string id);
    Task<Workspace?> GetByIngestKey(string organizationId, string ingestKey);
    Task<bool> NameExists(string organizationId, string name, string? exceptId = null);
    Task<Workspace> Create(string organizationId, Workspace workspace);
    Task<Workspace?> Update(string organizationId, string id, Workspace workspace);
    Task<Workspace?> Delete(string organizationId, string id);
}