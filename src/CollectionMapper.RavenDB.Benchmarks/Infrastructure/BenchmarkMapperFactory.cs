using System.Reflection;
using CollectionMapper.RavenDB.Models;

namespace CollectionMapper.RavenDB.Benchmarks.Infrastructure;

internal static class BenchmarkMapperFactory
{
    private static readonly FieldInfo MappedCollectionsField =
        typeof(RavenDBCollectionMapper)
            .GetField("_mappedCollections", BindingFlags.NonPublic | BindingFlags.Instance)!;

    public static RavenDBCollectionMapper CreatePopulated(Type[] types)
    {
        var mapper = new RavenDBCollectionMapper();
        var list = (List<RavenDBCollectionDefinitionModel>)MappedCollectionsField.GetValue(mapper)!;

        for (int i = 0; i < types.Length; i++)
            list.Add(new RavenDBCollectionDefinitionModel(types[i], $"Collection_{i}"));

        return mapper;
    }
}
