using LightOps.Mapping.Api.Services;

namespace LightOps.NeuralLens.OrganizationApi.Extensions;

public static class MappingServiceExtensions
{
    public static TDest? TryMap<TSource, TDest>(this IMappingService mappingService, TSource? source)
    {
        return source == null
            ? default
            : mappingService.Map<TSource, TDest>(source!);
    }

    public static IEnumerable<TDest?>? TryMap<TSource, TDest>(this IMappingService mappingService, IEnumerable<TSource?>? source)
    {
        return source?
            .Select(s => TryMap<TSource, TDest>(mappingService, s))
            .ToList();
    }
}