using LightOps.NeuralLens.Component.ObservabilityApiConnector.Codegen;
using Microsoft.Extensions.DependencyInjection;

namespace LightOps.NeuralLens.Component.ObservabilityApiConnector;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddObservabilityApiConnector(this IServiceCollection services, string baseAddress, string apiPrefix = "/")
    {
        // Add http client
        services.AddHttpClient<ObservabilityApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
        });

        // Add services
        services.AddSingleton<IObservabilityApiClient>(provider =>
        {
            var factory = provider.GetRequiredService<IHttpClientFactory>();
            return new ObservabilityApiClient(factory.CreateClient(nameof(ObservabilityApiClient)))
            {
                BaseUrl = apiPrefix,
            };
        });

        return services;
    }
}