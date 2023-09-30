using RavenDB.CollectionMapper.Exceptions;
using RavenDB.CollectionMapper.Tests.Common;
using RavenDB.CollectionMapper.Tests.Common.Classes;
using Xunit;

namespace RavenDB.CollectionMapper.Tests
{
    public class ExceptionTests
    {
        private readonly MyMapper _myMapper = new();

        [Fact]
        public void ShouldThrow_MappingAlreadyExistsException()
        {
            Assert.Throws<MappingAlreadyExistsException>(() => _myMapper.Map<Banana>("Banana").Map<Banana>("Apple"));
            Assert.Throws<MappingAlreadyExistsException>(() => _myMapper.Map<Apple>("X").Map<Apple>("X"));
            Assert.Throws<MappingAlreadyExistsException>(() => _myMapper.Map("Banana", typeof(Banana), typeof(Banana)));
            Assert.Throws<MappingAlreadyExistsException>(() => _myMapper.Map("Apple", typeof(Grape), typeof(Apple), typeof(Apple)));
        }

        [Fact]
        public void ShouldThrow_CannotMapAbstractClassesException()
        {
            Assert.Throws<CannotMapAbstractClassesException>(() => _myMapper.Map<Fruit>("Fruits"));
            Assert.Throws<CannotMapAbstractClassesException>(() => _myMapper.Map<Person>("People"));
            Assert.Throws<CannotMapAbstractClassesException>(() => _myMapper.Map<Vehicle>("Vehicles"));
            Assert.Throws<CannotMapAbstractClassesException>(() => _myMapper.Map("AbstractClasses", typeof(Fruit), typeof(Person), typeof(Vehicle)));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        public void ShouldThrow_CollectionNameCannotBeEmptyException(string collectionName)
        {
            Assert.Throws<CollectionNameCannotBeEmptyException>(() => _myMapper.Map(collectionName, typeof(Car)));
            Assert.Throws<CollectionNameCannotBeEmptyException>(() => _myMapper.Map<Car>(collectionName));
        }
    }
}
