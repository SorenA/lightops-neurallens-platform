namespace LightOps.NeuralLens.OrganizationApi.Requests;

public record UpdateOrganizationRequest(
    string Name,
    string? Description);