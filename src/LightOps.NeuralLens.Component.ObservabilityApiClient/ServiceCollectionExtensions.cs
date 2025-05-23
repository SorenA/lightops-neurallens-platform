using Microsoft.Extensions.DependencyInjection;

namespace LightOps.NeuralLens.Component.ObservabilityApiClient;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddObservabilityApiClient(this IServiceCollection services, string baseAddress, string apiPrefix = "/")
    {
        // Add http client
        services.AddHttpClient<Codegen.ObservabilityApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
        });

        // Add services
        services.AddSingleton(provider =>
        {
            var factory = provider.GetRequiredService<IHttpClientFactory>();
            return new Codegen.ObservabilityApiClient(apiPrefix, factory.CreateClient(nameof(Codegen.ObservabilityApiClient)));
        });

        return services;
    }
}