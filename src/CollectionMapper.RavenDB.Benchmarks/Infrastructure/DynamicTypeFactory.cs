using System.Reflection;
using System.Reflection.Emit;

namespace CollectionMapper.RavenDB.Benchmarks.Infrastructure;

internal static class DynamicTypeFactory
{
    private static readonly Dictionary<int, Type[]> _cache = new();

    public static Type[] GetTypes(int count)
    {
        if (_cache.TryGetValue(count, out var cached))
            return cached;

        var assemblyName = new AssemblyName($"BenchmarkDynamicTypes_{count}");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("BenchmarkModule");

        var types = new Type[count];
        for (int i = 0; i < count; i++)
        {
            var typeBuilder = moduleBuilder.DefineType(
                $"BenchmarkEntity_{count}_{i}",
                TypeAttributes.Public | TypeAttributes.Class);
            types[i] = typeBuilder.CreateType()!;
        }

        _cache[count] = types;
        return types;
    }
}
