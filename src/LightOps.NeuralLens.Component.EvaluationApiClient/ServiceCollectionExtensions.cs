using Microsoft.Extensions.DependencyInjection;

namespace LightOps.NeuralLens.Component.EvaluationApiClient;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEvaluationApiClient(this IServiceCollection services, string baseAddress, string apiPrefix = "/")
    {
        // Add http client
        services.AddHttpClient<Codegen.EvaluationApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
        });

        // Add services
        services.AddSingleton(provider =>
        {
            var factory = provider.GetRequiredService<IHttpClientFactory>();
            return new Codegen.EvaluationApiClient(apiPrefix, factory.CreateClient(nameof(Codegen.EvaluationApiClient)));
        });

        return services;
    }
}