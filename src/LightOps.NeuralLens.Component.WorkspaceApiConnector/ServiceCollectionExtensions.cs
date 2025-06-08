using LightOps.NeuralLens.Component.WorkspaceApiConnector.Codegen;
using Microsoft.Extensions.DependencyInjection;

namespace LightOps.NeuralLens.Component.WorkspaceApiConnector;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkspaceApiConnector(this IServiceCollection services, string baseAddress, string apiPrefix = "/")
    {
        // Add http client
        services.AddHttpClient<WorkspaceApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
        });

        // Add services
        services.AddSingleton<IWorkspaceApiClient>(provider =>
        {
            var factory = provider.GetRequiredService<IHttpClientFactory>();
            return new WorkspaceApiClient(factory.CreateClient(nameof(WorkspaceApiClient)))
            {
                BaseUrl = apiPrefix,
            };
        });

        return services;
    }
}