using Microsoft.Extensions.DependencyInjection;

namespace LightOps.NeuralLens.Component.OrganizationApiClient;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrganizationApiClient(this IServiceCollection services, string baseAddress, string apiPrefix = "/")
    {
        // Add http client
        services.AddHttpClient<Codegen.OrganizationApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
        });

        // Add services
        services.AddSingleton(provider =>
        {
            var factory = provider.GetRequiredService<IHttpClientFactory>();
            return new Codegen.OrganizationApiClient(apiPrefix, factory.CreateClient(nameof(Codegen.OrganizationApiClient)));
        });

        return services;
    }
}