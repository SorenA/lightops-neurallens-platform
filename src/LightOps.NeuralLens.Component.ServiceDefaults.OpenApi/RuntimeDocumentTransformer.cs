using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace LightOps.NeuralLens.Component.ServiceDefaults.OpenApi;

public class RuntimeDocumentTransformer : IOpenApiDocumentTransformer
{
    private OpenApiInfo? _apiInfo;
    private readonly List<OpenApiServer> _servers = [];

    public RuntimeDocumentTransformer WithInfo(string title, string? description = null, string version = "v1")
    {
        _apiInfo = new()
        {
            Title = title,
            Description = description,
            Version = version,
        };
        return this;
    }

    public RuntimeDocumentTransformer AddServer(string url, string? description = null)
    {
        _servers.Add(new()
        {
            Url = url,
            Description = description ?? string.Empty,
        });
        return this;
    }

    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (_apiInfo != null)
        {
            document.Info = _apiInfo;
        }

        _servers.ForEach(x => document.Servers.Add(x));

        return Task.CompletedTask;
    }
}