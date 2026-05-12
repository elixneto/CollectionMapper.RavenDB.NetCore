using CollectionMapper.RavenDB.Attributes;
using CollectionMapper.RavenDB.Exceptions;
using CollectionMapper.RavenDB.Tests.Fixtures;
using Xunit;

namespace CollectionMapper.RavenDB.Tests;

public class RavenDBEmptyCollectionNameExceptionTests
{
    [Fact]
    public void Exception_InheritsFromException()
    {
        var ex = new RavenDBEmptyCollectionNameException();
        Assert.IsAssignableFrom<Exception>(ex);
    }

    [Fact]
    public void Exception_HasExpectedMessage()
    {
        var ex = new RavenDBEmptyCollectionNameException();
        Assert.Equal("Collection name must not be empty.", ex.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("\t")]
    public void RavenCollectionAttribute_EmptyOrWhitespaceName_ThrowsRavenDBEmptyCollectionNameException(string name)
    {
        Assert.Throws<RavenDBEmptyCollectionNameException>(() => new RavenCollectionAttribute(name));
    }

    [Fact]
    public void RavenCollectionAttribute_NullName_ThrowsRavenDBEmptyCollectionNameException()
    {
        Assert.Throws<RavenDBEmptyCollectionNameException>(() => new RavenCollectionAttribute(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("\t")]
    public void Map_EmptyOrWhitespaceName_ThrowsRavenDBEmptyCollectionNameException(string name)
    {
        var mapper = new RavenDBCollectionMapper();
        
        Assert.Throws<RavenDBEmptyCollectionNameException>(() => mapper.Map<Banana>(name));
    }

    [Fact]
    public void Map_NullName_ThrowsRavenDBEmptyCollectionNameException()
    {
        var mapper = new RavenDBCollectionMapper();
        
        Assert.Throws<RavenDBEmptyCollectionNameException>(() => mapper.Map<Banana>(null!));
    }
}
