using CollectionMapper.RavenDB.Attributes;
using CollectionMapper.RavenDB.Exceptions;
using CollectionMapper.RavenDB.Tests.Fixtures;
using CollectionMapper.RavenDB.Tests.Helpers;
using Xunit;

namespace CollectionMapper.RavenDB.Tests;

[Collection("Serial")]
public class RavenCollectionAttributeTests
{
    [Fact]
    public void FindCollection_WithRavenCollectionAttribute_ReturnsAttributeValue()
    {
        RegistryResetHelper.Reset();
        RavenDBMapperConventions.RegisterByAssembly<Fruit>();

        Assert.Equal("Fruits", RavenDBMapperConventions.FindCollection(typeof(Fruit)));
    }

    [Fact]
    public void RavenCollectionAttribute_BlankName_ThrowsRavenDBEmptyCollectionNameException()
    {
        Assert.Throws<RavenDBEmptyCollectionNameException>(() => new RavenCollectionAttribute("  "));
    }

    [Fact]
    public void RavenCollectionAttribute_NullName_ThrowsRavenDBEmptyCollectionNameException()
    {
        Assert.Throws<RavenDBEmptyCollectionNameException>(() => new RavenCollectionAttribute(null!));
    }

    [Fact]
    public void RavenCollectionAttribute_StoresCollectionName()
    {
        var attr = new RavenCollectionAttribute("TestCollection");
        Assert.Equal("TestCollection", attr.CollectionName);
    }
}
