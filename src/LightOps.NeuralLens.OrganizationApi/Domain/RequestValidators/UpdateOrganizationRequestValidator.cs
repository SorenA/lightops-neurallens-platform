using FluentValidation;
using FluentValidation.Results;
using LightOps.NeuralLens.OrganizationApi.Domain.Repositories;
using LightOps.NeuralLens.OrganizationApi.Requests;
using Microsoft.AspNetCore.Mvc.Filters;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Interceptors;

namespace LightOps.NeuralLens.OrganizationApi.Domain.RequestValidators;

public class UpdateOrganizationRequestValidator : AbstractValidator<UpdateOrganizationRequest>, IValidatorInterceptor
{
    private readonly IOrganizationRepository _organizationRepository;
    private string? _organizationId;

    public UpdateOrganizationRequestValidator(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(256)
            .MustAsync((s, _) => IsNameUnique(s)).WithMessage("Name already taken.");
    }

    private async Task<bool> IsNameUnique(string name)
    {
        return !await _organizationRepository.NameExists(name, _organizationId);
    }

    public IValidationContext? BeforeValidation(ActionExecutingContext actionExecutingContext, IValidationContext validationContext)
    {
        // Read id argument from endpoint
        actionExecutingContext.ActionArguments.TryGetValue("id", out var id);
        _organizationId = id as string;

        return null;
    }

    public ValidationResult? AfterValidation(ActionExecutingContext actionExecutingContext, IValidationContext validationContext)
    {
        return null;
    }
}