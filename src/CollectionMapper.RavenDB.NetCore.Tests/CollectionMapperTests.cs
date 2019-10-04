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
        public void Should_return_the_right_collection_name_by_type(string expected, Type type)
        {
            var actualCollectionName = _myMapper.FindCollectionBy(type);

            Assert.Equal(expected, actualCollectionName);
        }

        [Fact]
        public void Should_merge_two_different_mappers()
        {
            var anotherMapper = new MyMapper().Map<Car>("Cars");

            _myMapper.Merge(anotherMapper);
            var totalOfMappings = _myMapper.Collections.Count;

            Assert.Equal(6, totalOfMappings);
            Assert.Equal(1, anotherMapper.Collections.Count);
        }

        [Theory]
        [InlineData(typeof(Apple))]
        [InlineData(typeof(Banana))]
        [InlineData(typeof(Man))]
        [InlineData(typeof(Woman))]
        public void Should_return_true_when_the_class_is_mapped(Type type)
        {
            var isMapped = _myMapper.IsMappedBy(type);

            Assert.True(isMapped);
        }

        [Theory]
        [InlineData(typeof(Car))]
        [InlineData(typeof(Tractor))]
        public void Should_return_false_when_the_class_is_not_mapped(Type type)
        {
            var isMapped = _myMapper.IsMappedBy(type);

            Assert.False(isMapped);
        }

        [Fact]
        public void Should_not_throw_exceptions_with_fluent_mapping()
        {
            var newMapper = new MyMapper();
            newMapper.Map<Banana>("Fruit").Map<Apple>("Fruit");
            newMapper.Map<Grape>("Fruit");
        }

        [Fact]
        public void Should_not_throw_exceptions_with_param_vector_mapping()
        {
            var newMapper = new MyMapper();
            newMapper.Map("Fruit", typeof(Banana), typeof(Apple));
            newMapper.Map("Fruit", typeof(Grape));
        }
    }
}
