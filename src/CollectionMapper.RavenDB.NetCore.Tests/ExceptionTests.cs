using CollectionMapper.RavenDB.NetCore.Exceptions;
using CollectionMapper.RavenDB.NetCore.Tests.Common;
using CollectionMapper.RavenDB.NetCore.Tests.Common.Classes;
using Xunit;

namespace CollectionMapper.RavenDB.NetCore.Tests
{
    public class ExceptionTests
    {
        private readonly MyMapper _myMapper = new MyMapper();

        [Fact]
        public void Should_throw_mapping_already_exists_with_fluent_mapping()
        {
            Assert.Throws<MappingAlreadyExistsException>(() => _myMapper.Map<Banana>("Banana").Map<Banana>("Apple"));
            Assert.Throws<MappingAlreadyExistsException>(() => _myMapper.Map<Apple>("X").Map<Apple>("X"));
        }

        [Fact]
        public void Should_throw_mapping_already_exists_with_param_vector_mapping()
        {
            Assert.Throws<MappingAlreadyExistsException>(() => _myMapper.Map("Banana", typeof(Banana), typeof(Banana)));
            Assert.Throws<MappingAlreadyExistsException>(() => _myMapper.Map("Apple", typeof(Grape), typeof(Apple), typeof(Apple)));
        }

        [Fact]
        public void Should_throw_cannot_map_abstract_classes_exception_with_fluent_mapping()
        {
            Assert.Throws<CannotMapAbstractClassesException>(() => _myMapper.Map<Fruit>("Fruits"));
            Assert.Throws<CannotMapAbstractClassesException>(() => _myMapper.Map<Person>("People"));
            Assert.Throws<CannotMapAbstractClassesException>(() => _myMapper.Map<Vehicle>("Vehicles"));
        }

        [Fact]
        public void Should_throw_cannot_map_abstract_classes_exception_with_param_vector_mapping()
        {
            Assert.Throws<CannotMapAbstractClassesException>(() => _myMapper.Map("AbstractClasses", typeof(Fruit), typeof(Person), typeof(Vehicle)));
        }
    }
}
