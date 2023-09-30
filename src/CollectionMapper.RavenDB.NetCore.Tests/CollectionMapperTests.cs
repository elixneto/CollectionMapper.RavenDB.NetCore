using CollectionMapper.RavenDB.NetCore.Tests.Common;
using CollectionMapper.RavenDB.NetCore.Tests.Common.Classes;
using System;
using Xunit;

namespace CollectionMapper.RavenDB.NetCore.Tests
{
    public class CollectionMapperTests
    {
        private readonly MyMapper _myMapper = MyMapperBuilder.Build();

        [Theory]
        [InlineData("People", typeof(Man))]
        [InlineData("People", typeof(Woman))]
        [InlineData("Fruits", typeof(Apple))]
        [InlineData("Fruits", typeof(Banana))]
        public void Should_ReturnTheRightCollectionNameByType(string expected, Type type)
        {
            var actualCollectionName = _myMapper.FindCollectionBy(type);

            Assert.Equal(expected, actualCollectionName);
        }

        [Fact]
        public void Should_MergeTwoDifferentMappers()
        {
            var anotherMapper = new MyMapper().Map<Car>("Cars");

            _myMapper.Merge(anotherMapper);
            var totalOfMappings = _myMapper.GetMappedCollections().Count;

            Assert.Equal(6, totalOfMappings);
            Assert.Equal(1, anotherMapper.GetMappedCollections().Count);
        }

        [Theory]
        [InlineData(typeof(Apple))]
        [InlineData(typeof(Banana))]
        [InlineData(typeof(Man))]
        [InlineData(typeof(Woman))]
        public void Should_ReturnTrue_WhenTheClassHasMapping(Type type)
        {
            var isMapped = _myMapper.HasMappingFor(type);

            Assert.True(isMapped);
        }

        [Theory]
        [InlineData(typeof(Car))]
        [InlineData(typeof(Tractor))]
        public void Should_ReturnTrue_WhenTheClassDoesNotHaveMapping(Type type)
        {
            var isMapped = _myMapper.HasMappingFor(type);

            Assert.False(isMapped);
        }

        [Fact]
        public void Should_NotThrowExceptions_WithFluentMapping()
        {
            var newMapper = new MyMapper();
            newMapper.Map<Banana>("Fruit").Map<Apple>("Fruit");
            newMapper.Map<Grape>("Fruit");

            Assert.True(newMapper.HasMappingFor<Banana>());
            Assert.False(newMapper.HasMappingFor<Car>());
        }

        [Fact]
        public void Should_NotThrowExceptions_WithParamVectorMapping()
        {
            var newMapper = new MyMapper();
            newMapper.Map("Fruit", typeof(Banana), typeof(Apple));
            newMapper.Map("Fruit", typeof(Grape));

            Assert.True(newMapper.HasMappingFor<Banana>());
            Assert.False(newMapper.HasMappingFor<Car>());
        }
    }
}
