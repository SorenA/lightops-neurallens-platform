namespace LightOps.NeuralLens.OrganizationApi.Requests;

public record CreateOrganizationRequest(
    string Name,
    string? Description);