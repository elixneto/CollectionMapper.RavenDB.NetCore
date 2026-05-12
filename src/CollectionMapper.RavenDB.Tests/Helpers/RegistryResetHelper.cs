using System.Collections.Concurrent;
using System.Reflection;

namespace CollectionMapper.RavenDB.Tests.Helpers;

internal static class RegistryResetHelper
{
    private static readonly Type ConventionsType = typeof(RavenDBMapperConventions);

    public static void Reset()
    {
        var registryField = ConventionsType.GetField("_collectionsRegistry", BindingFlags.NonPublic | BindingFlags.Static)!;
        var dict = (ConcurrentDictionary<Type, string>)registryField.GetValue(null)!;
        dict.Clear();

        SetBool("_isDebugModeEnabled", false);
    }

    private static void SetBool(string fieldName, bool value) =>
        ConventionsType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static)!.SetValue(null, value);
}
