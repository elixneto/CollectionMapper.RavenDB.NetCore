using CollectionMapper.RavenDB.Exceptions;
using CollectionMapper.RavenDB.Tests.Fixtures;
using CollectionMapper.RavenDB.Tests.Helpers;
using Xunit;

namespace CollectionMapper.RavenDB.Tests;

[Collection("Serial")]
public class RavenDBCollectionMapperTests
{
    [Fact]
    public void Map_RegistersTypeInMappedCollections()
    {
        var mapper = new RavenDBCollectionMapper();
        mapper.Map<Banana>("Fruits");

        Assert.Single(mapper.MappedCollections);
        Assert.Equal(typeof(Banana), mapper.MappedCollections[0].CollectionType);
        Assert.Equal("Fruits", mapper.MappedCollections[0].CollectionName);
    }

    [Fact]
    public void Map_IsFluent_AllowsChaining()
    {
        var mapper = new RavenDBCollectionMapper();
        var returned = mapper.Map<Fruit>("Fruits").Map<Banana>("Fruits");

        Assert.Same(mapper, returned);
        Assert.Equal(2, mapper.MappedCollections.Count);
    }

    [Fact]
    public void Map_DuplicateType_LastWriteWins()
    {
        var mapper = new RavenDBCollectionMapper();
        mapper.Map<Banana>("OldName").Map<Banana>("Fruits");

        Assert.Single(mapper.MappedCollections);
        Assert.Equal("Fruits", mapper.MappedCollections[0].CollectionName);
    }

    [Fact]
    public void FindCollection_AfterRegisterMapper_ReturnsCorrectName()
    {
        RegistryResetHelper.Reset();
        var mapper = new RavenDBCollectionMapper();
        mapper.Map<Banana>("Fruits");
        RavenDBMapperConventions.RegisterMapper(mapper);

        Assert.Equal("Fruits", RavenDBMapperConventions.FindCollection(typeof(Banana)));
    }

    [Fact]
    public void Map_EmptyCollectionName_ThrowsRavenDBEmptyCollectionNameException()
    {
        var mapper = new RavenDBCollectionMapper();
        Assert.Throws<RavenDBEmptyCollectionNameException>(() => mapper.Map<Banana>(""));
    }

    [Fact]
    public void Map_WhitespaceCollectionName_ThrowsRavenDBEmptyCollectionNameException()
    {
        var mapper = new RavenDBCollectionMapper();
        Assert.Throws<RavenDBEmptyCollectionNameException>(() => mapper.Map<Banana>("  "));
    }

    [Fact]
    public void RegisterMapper_SameType_DifferentName_OverridesPreviousRegistration()
    {
        RegistryResetHelper.Reset();
        var m1 = new RavenDBCollectionMapper();
        m1.Map<Banana>("OldName");
        var m2 = new RavenDBCollectionMapper();
        m2.Map<Banana>("NewName");

        RavenDBMapperConventions.RegisterMapper(m1);
        RavenDBMapperConventions.RegisterMapper(m2);

        Assert.Equal("NewName", RavenDBMapperConventions.FindCollection(typeof(Banana)));
    }

    [Fact]
    public void RegisterMapper_SameType_SameName_NoChange()
    {
        RegistryResetHelper.Reset();
        var mapper = new RavenDBCollectionMapper();
        mapper.Map<Banana>("Fruits");

        RavenDBMapperConventions.RegisterMapper(mapper);
        RavenDBMapperConventions.RegisterMapper(mapper);

        Assert.Equal("Fruits", RavenDBMapperConventions.FindCollection(typeof(Banana)));
    }
}
