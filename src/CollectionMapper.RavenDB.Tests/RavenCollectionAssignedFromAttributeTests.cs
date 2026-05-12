using CollectionMapper.RavenDB.Attributes;
using CollectionMapper.RavenDB.Tests.Fixtures;
using CollectionMapper.RavenDB.Tests.Helpers;
using Raven.Client.Documents.Conventions;
using Xunit;

namespace CollectionMapper.RavenDB.Tests;

[Collection("Serial")]
public class RavenCollectionAssignedFromAttributeTests
{
    [Fact]
    public void FindCollection_AppleWithAssignedFromFruit_ReturnsFruitsCollection()
    {
        RegistryResetHelper.Reset();
        RavenDBMapperConventions.RegisterByAssembly<Fruit>();

        Assert.Equal("Fruits", RavenDBMapperConventions.FindCollection(typeof(Apple)));
    }

    [Fact]
    public void FindCollection_GrapeWithAssignedFromFruit_ReturnsFruitsCollection()
    {
        RegistryResetHelper.Reset();
        RavenDBMapperConventions.RegisterByAssembly<Fruit>();

        Assert.Equal("Fruits", RavenDBMapperConventions.FindCollection(typeof(Grape)));
    }

    [Fact]
    public void AssignedFromAttribute_BaseType_ReturnsCorrectType()
    {
        var attr = new RavenCollectionAssignedFromAttribute<Fruit>();
        Assert.Equal(typeof(Fruit), attr.BaseType);
    }

    [Fact]
    public void FindCollection_Apple_AndGrape_MapToSameCollection()
    {
        RegistryResetHelper.Reset();
        RavenDBMapperConventions.RegisterByAssembly<Fruit>();

        var apple = RavenDBMapperConventions.FindCollection(typeof(Apple));
        var grape = RavenDBMapperConventions.FindCollection(typeof(Grape));

        Assert.Equal(apple, grape);
    }

    [Fact]
    public void FindCollection_AssignedFromUnattributedBase_FallsBackToDefaultNaming()
    {
        RegistryResetHelper.Reset();
        RavenDBMapperConventions.RegisterByAssembly<Fruit>();

        var expected = DocumentConventions.DefaultGetCollectionName(typeof(VehicleBase));
        Assert.Equal(expected, RavenDBMapperConventions.FindCollection(typeof(Car)));
    }
}
