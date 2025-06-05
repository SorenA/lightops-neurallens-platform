using LightOps.NeuralLens.Component.ServiceDefaults;

namespace LightOps.NeuralLens.WorkspaceApi.Domain.Services;

public class IngestKeyService()
{
    public string GenerateKey() => $"{ApiKeyPrefixes.IngestKey}{Guid.NewGuid()}";
    public bool IsFormatValid(string ingestKey) => ingestKey.StartsWith(ApiKeyPrefixes.IngestKey);
}