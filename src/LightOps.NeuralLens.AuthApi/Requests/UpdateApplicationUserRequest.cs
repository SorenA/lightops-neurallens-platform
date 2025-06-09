namespace LightOps.NeuralLens.AuthApi.Requests;

public record UpdateApplicationUserRequest(
    string? Name,
    string? PictureUrl,
    string? ExternalProvider,
    string? ExternalId);