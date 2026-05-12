using CollectionMapper.RavenDB.Tests.Fixtures;
using CollectionMapper.RavenDB.Tests.Helpers;
using Raven.Client.Documents.Conventions;
using Xunit;

namespace CollectionMapper.RavenDB.Tests;

[Collection("Serial")]
public class FindCollectionFallbackTests
{
    [Fact]
    public void FindCollection_UnknownType_FallsBackToRavenDBDefault()
    {
        RegistryResetHelper.Reset();

        var result = RavenDBMapperConventions.FindCollection(typeof(UnmappedDocument));
        var expected = DocumentConventions.DefaultGetCollectionName(typeof(UnmappedDocument));

        Assert.Equal(expected, result);
    }

    [Fact]
    public void FindCollection_RegisteredThenReset_FallsBackToDefault()
    {
        RegistryResetHelper.Reset();
        RavenDBMapperConventions.RegisterByAssembly<Fruit>();
        RegistryResetHelper.Reset();

        var result = RavenDBMapperConventions.FindCollection(typeof(Fruit));
        var expected = DocumentConventions.DefaultGetCollectionName(typeof(Fruit));

        Assert.Equal(expected, result);
    }

    [Fact]
    public void RegisterByAssemblies_CalledWithMultipleAssemblies_RegistersAll()
    {
        RegistryResetHelper.Reset();
        var asm = typeof(Fruit).Assembly;

        RavenDBMapperConventions.RegisterByAssemblies(asm, asm);

        Assert.Equal("Fruits", RavenDBMapperConventions.FindCollection(typeof(Fruit)));
        Assert.Equal("Fruits", RavenDBMapperConventions.FindCollection(typeof(Apple)));
    }
}
