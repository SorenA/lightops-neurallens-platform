using FluentValidation;
using FluentValidation.Results;
using LightOps.NeuralLens.WorkspaceApi.Domain.Repositories;
using LightOps.NeuralLens.WorkspaceApi.Requests;
using Microsoft.AspNetCore.Mvc.Filters;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Interceptors;

namespace LightOps.NeuralLens.WorkspaceApi.Domain.RequestValidators;

public class UpdateWorkspaceRequestValidator : AbstractValidator<UpdateWorkspaceRequest>, IValidatorInterceptor
{
    private readonly IWorkspaceRepository _workspaceRepository;
    private string? _workspaceId;
    private string? _organizationId;

    public UpdateWorkspaceRequestValidator(IWorkspaceRepository workspaceRepository)
    {
        _workspaceRepository = workspaceRepository;
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(256)
            .MustAsync((s, _) => IsNameUnique(s)).WithMessage("Name already taken.");
    }

    private async Task<bool> IsNameUnique(string name)
    {
        return !await _workspaceRepository.NameExists(_organizationId ?? string.Empty, name, _workspaceId);
    }

    public IValidationContext? BeforeValidation(ActionExecutingContext actionExecutingContext, IValidationContext validationContext)
    {
        // Read id argument from endpoint
        actionExecutingContext.ActionArguments.TryGetValue("id", out var id);
        actionExecutingContext.ActionArguments.TryGetValue("organizationId", out var organizationId);
        _workspaceId = id as string;
        _organizationId = organizationId as string;

        return null;
    }

    public ValidationResult? AfterValidation(ActionExecutingContext actionExecutingContext, IValidationContext validationContext)
    {
        return null;
    }
}