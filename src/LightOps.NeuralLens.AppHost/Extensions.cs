using Microsoft.Extensions.Hosting;

namespace LightOps.NeuralLens.AppHost
{
    public static class Extensions
    {
        public static IResourceBuilder<NodeAppResource> AddTurboRepoProject(this IDistributedApplicationBuilder builder, string name, string workingDirectory, string packageManager = "npm", string scriptName = "start", string[]? args = null)
        {
            string[] allArgs = args is { Length: > 0 }
                ? ["run", scriptName, "--", .. args]
                : ["run", scriptName];

            var resource = new NodeAppResource(name, packageManager, workingDirectory);
            return builder.AddResource(resource)
                .WithOtlpExporter()
                .WithEnvironment("NODE_ENV", builder.Environment.IsDevelopment() ? "development" : "production")
                .WithArgs(allArgs);
        }
    }
}
