using LightOps.NeuralLens.WorkspaceApi.Domain.Models;
using LightOps.NeuralLens.WorkspaceApi.Domain.Repositories;
using LightOps.NeuralLens.WorkspaceApi.Requests;

namespace LightOps.NeuralLens.WorkspaceApi.Domain.Services;

public class WorkspaceService(
    IWorkspaceRepository workspaceRepository,
    IngestKeyService ingestKeyService)
{
    public async Task<List<Workspace>> GetAll(string organizationId)
    {
        return await workspaceRepository.GetAll(organizationId);
    }

    public async Task<Workspace?> GetById(string organizationId, string id)
    {
        return await workspaceRepository.GetById(organizationId, id);
    }

    public async Task<Workspace?> GetByIngestKey(string organizationId, string ingestKey)
    {
        return await workspaceRepository.GetByIngestKey(organizationId, ingestKey);
    }

    public async Task<Workspace> Create(string organizationId, CreateWorkspaceRequest request)
    {
        // Map request to domain model
        var entity = new Workspace(
            Guid.NewGuid().ToString(),
            organizationId,
            request.Name,
            ingestKeyService.GenerateKey(),
            DateTime.UtcNow,
            DateTime.UtcNow)
        {
            Description = request.Description ?? string.Empty,
        };

        return await workspaceRepository.Create(organizationId, entity);
    }

    public async Task<Workspace?> Update(string organizationId, string id, UpdateWorkspaceRequest request)
    {
        var entity = await workspaceRepository.GetById(organizationId, id);
        if (entity == null)
        {
            return null;
        }

        // Map request to domain model
        entity.Name = request.Name;
        entity.Description = request.Description ?? string.Empty;
        entity.UpdatedAt = DateTime.UtcNow;

        return await workspaceRepository.Update(organizationId, id, entity);
    }

    public async Task<Workspace?> Delete(string organizationId, string id)
    {
        var entity = await workspaceRepository.GetById(organizationId, id);
        if (entity == null)
        {
            return null;
        }

        // Soft-delete
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;

        return await workspaceRepository.Update(organizationId, id, entity);
    }
}