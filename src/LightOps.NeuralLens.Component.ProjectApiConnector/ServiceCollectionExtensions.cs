using LightOps.NeuralLens.Component.ProjectApiConnector.Codegen;
using Microsoft.Extensions.DependencyInjection;

namespace LightOps.NeuralLens.Component.ProjectApiConnector;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProjectApiConnector(this IServiceCollection services, string baseAddress, string apiPrefix = "/")
    {
        // Add http client
        services.AddHttpClient<ProjectApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
        });

        // Add services
        services.AddSingleton<IProjectApiClient>(provider =>
        {
            var factory = provider.GetRequiredService<IHttpClientFactory>();
            return new ProjectApiClient(apiPrefix, factory.CreateClient(nameof(ProjectApiClient)));
        });

        return services;
    }
}