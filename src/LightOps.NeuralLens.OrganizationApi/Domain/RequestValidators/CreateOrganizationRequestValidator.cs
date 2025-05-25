using FluentValidation;
using LightOps.NeuralLens.OrganizationApi.Domain.Repositories;
using LightOps.NeuralLens.OrganizationApi.Requests;

namespace LightOps.NeuralLens.OrganizationApi.Domain.RequestValidators;

public class CreateOrganizationRequestValidator : AbstractValidator<CreateOrganizationRequest>
{
    private readonly IOrganizationRepository _organizationRepository;

    public CreateOrganizationRequestValidator(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(256)
            .MustAsync((s, _) => IsNameUnique(s)).WithMessage("Name already taken.");
    }

    private async Task<bool> IsNameUnique(string name)
    {
        return !await _organizationRepository.NameExists(name);
    }
}