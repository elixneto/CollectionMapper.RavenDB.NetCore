using System.Collections.Concurrent;
using System.Reflection;
using CollectionMapper.RavenDB;

namespace CollectionMapper.RavenDB.Benchmarks.Infrastructure;

internal static class RegistryResetHelper
{
    private static readonly Type ConventionsType = typeof(RavenDBMapperConventions);

    private static readonly FieldInfo RegistryField =
        ConventionsType.GetField("_collectionsRegistry", BindingFlags.NonPublic | BindingFlags.Static)!;

    private static readonly FieldInfo DebugField =
        ConventionsType.GetField("_isDebugModeEnabled", BindingFlags.NonPublic | BindingFlags.Static)!;

    public static void Reset()
    {
        var dict = (ConcurrentDictionary<Type, string>)RegistryField.GetValue(null)!;
        dict.Clear();
        DebugField.SetValue(null, false);
    }
}
