namespace LightOps.NeuralLens.WorkspaceApi.Domain.Services;

public class IngestKeyService()
{
    /*
     * ik = Ingest Key
     * nl = Neural Lens
     */
    private const string KeyPrefix = "ik-nl-";

    public string GenerateKey() => $"{KeyPrefix}{Guid.NewGuid()}";
    public bool IsFormatValid(string ingestKey) => ingestKey.StartsWith(KeyPrefix);
}