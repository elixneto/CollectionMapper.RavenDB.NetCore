using System.Reflection;
using CollectionMapper.RavenDB.Tests.Fixtures;
using CollectionMapper.RavenDB.Tests.Helpers;
using Xunit;

namespace CollectionMapper.RavenDB.Tests;

[Collection("Serial")]
public class AssemblyScanTests
{
    [Fact]
    public void RegisterByAssembly_ConcreteMapperSubclass_RegistersMappedTypes()
    {
        RegistryResetHelper.Reset();
        RavenDBMapperConventions.RegisterByAssembly<Fruit>();

        Assert.Equal("ScannedBananas", RavenDBMapperConventions.FindCollection(typeof(Banana)));
    }

    [Fact]
    public void ResolveCollectionName_TypeWithRavenCollectionAttribute_NotInRegistry_ReturnsAttributeValue()
    {
        RegistryResetHelper.Reset();

        var method = typeof(RavenDBMapperConventions)
            .GetMethod("ResolveCollectionName", BindingFlags.NonPublic | BindingFlags.Static)!;

        var result = (string)method.Invoke(null, [typeof(Fruit)])!;

        Assert.Equal("Fruits", result);
    }

    [Fact]
    public void RegisterByAssemblies_PartialTypeLoadFailure_DoesNotThrow()
    {
        RegistryResetHelper.Reset();
        var brokenAssembly = BrokenAssemblyHelper.Create();

        var exception = Record.Exception(
            () => RavenDBMapperConventions.RegisterByAssemblies(brokenAssembly));

        Assert.Null(exception);
    }
}
