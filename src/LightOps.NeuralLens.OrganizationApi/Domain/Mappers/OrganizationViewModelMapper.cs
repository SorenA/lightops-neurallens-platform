using LightOps.Mapping.Api.Mappers;
using LightOps.NeuralLens.OrganizationApi.Domain.Models;
using LightOps.NeuralLens.OrganizationApi.Models;

namespace LightOps.NeuralLens.OrganizationApi.Domain.Mappers;

public class OrganizationViewModelMapper : IMapper<Organization, OrganizationViewModel>
{
    public OrganizationViewModel Map(Organization source)
    {
        return new OrganizationViewModel(
            source.Id,
            source.Name,
            source.CreatedAt,
            source.UpdatedAt)
        {
            Description = source.Description,
        };
    }
}