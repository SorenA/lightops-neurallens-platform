using Microsoft.Extensions.DependencyInjection;

namespace LightOps.NeuralLens.Component.ProjectApiClient;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProjectApiClient(this IServiceCollection services, string baseAddress, string apiPrefix = "/")
    {
        // Add http client
        services.AddHttpClient<Codegen.ProjectApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
        });

        // Add services
        services.AddSingleton(provider =>
        {
            var factory = provider.GetRequiredService<IHttpClientFactory>();
            return new Codegen.ProjectApiClient(apiPrefix, factory.CreateClient(nameof(Codegen.ProjectApiClient)));
        });

        return services;
    }
}