using LightOps.NeuralLens.Component.EvaluationApiConnector.Codegen;
using Microsoft.Extensions.DependencyInjection;

namespace LightOps.NeuralLens.Component.EvaluationApiConnector;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEvaluationApiConnector(this IServiceCollection services, string baseAddress, string apiPrefix = "/")
    {
        // Add http client
        services.AddHttpClient<EvaluationApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
        });

        // Add services
        services.AddSingleton<IEvaluationApiClient>(provider =>
        {
            var factory = provider.GetRequiredService<IHttpClientFactory>();
            return new EvaluationApiClient(apiPrefix, factory.CreateClient(nameof(EvaluationApiClient)));
        });

        return services;
    }
}