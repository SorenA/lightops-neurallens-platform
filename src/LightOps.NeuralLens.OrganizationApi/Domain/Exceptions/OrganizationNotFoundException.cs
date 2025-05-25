namespace LightOps.NeuralLens.OrganizationApi.Domain.Exceptions;

internal class OrganizationNotFoundException(string organizationId)
    : Exception($"Organization with ID {organizationId} not found.")
{
    public string OrganizationId { get; } = organizationId;
}