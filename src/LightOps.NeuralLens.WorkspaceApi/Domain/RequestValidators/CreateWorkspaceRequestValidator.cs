using FluentValidation;
using FluentValidation.Results;
using LightOps.NeuralLens.WorkspaceApi.Domain.Repositories;
using LightOps.NeuralLens.WorkspaceApi.Requests;
using Microsoft.AspNetCore.Mvc.Filters;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Interceptors;

namespace LightOps.NeuralLens.WorkspaceApi.Domain.RequestValidators;

public class CreateWorkspaceRequestValidator : AbstractValidator<CreateWorkspaceRequest>, IValidatorInterceptor
{
    private readonly IWorkspaceRepository _workspaceRepository;
    private string? _organizationId;

    public CreateWorkspaceRequestValidator(IWorkspaceRepository workspaceRepository)
    {
        _workspaceRepository = workspaceRepository;
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(256)
            .MustAsync((s, _) => IsNameUnique(s)).WithMessage("Name already taken.");
    }

    private async Task<bool> IsNameUnique(string name)
    {
        return !await _workspaceRepository.NameExists(_organizationId ?? string.Empty, name);
    }

    public IValidationContext? BeforeValidation(ActionExecutingContext actionExecutingContext, IValidationContext validationContext)
    {
        // Read id argument from endpoint
        actionExecutingContext.ActionArguments.TryGetValue("organizationId", out var organizationId);
        _organizationId = organizationId as string;

        return null;
    }

    public ValidationResult? AfterValidation(ActionExecutingContext actionExecutingContext, IValidationContext validationContext)
    {
        return null;
    }
}