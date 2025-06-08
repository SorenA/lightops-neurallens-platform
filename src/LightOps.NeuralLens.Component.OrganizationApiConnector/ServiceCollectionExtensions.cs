using LightOps.NeuralLens.Component.OrganizationApiConnector.Codegen;
using Microsoft.Extensions.DependencyInjection;

namespace LightOps.NeuralLens.Component.OrganizationApiConnector;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrganizationApiConnector(this IServiceCollection services, string baseAddress, string apiPrefix = "/")
    {
        // Add http client
        services.AddHttpClient<OrganizationApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
        });

        // Add services
        services.AddSingleton<IOrganizationApiClient>(provider =>
        {
            var factory = provider.GetRequiredService<IHttpClientFactory>();
            return new OrganizationApiClient(factory.CreateClient(nameof(OrganizationApiClient)))
            {
                BaseUrl = apiPrefix,
            };
        });

        return services;
    }
}