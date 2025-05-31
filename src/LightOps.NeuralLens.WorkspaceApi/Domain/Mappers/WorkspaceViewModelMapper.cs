using LightOps.Mapping.Api.Mappers;
using LightOps.NeuralLens.WorkspaceApi.Domain.Models;
using LightOps.NeuralLens.WorkspaceApi.Models;

namespace LightOps.NeuralLens.WorkspaceApi.Domain.Mappers;

public class WorkspaceViewModelMapper : IMapper<Workspace, WorkspaceViewModel>
{
    public WorkspaceViewModel Map(Workspace source)
    {
        return new WorkspaceViewModel(
            source.Id,
            source.OrganizationId,
            source.Name,
            source.IngestKey,
            source.CreatedAt,
            source.UpdatedAt)
        {
            Description = source.Description,
        };
    }
}