using RavenDB.CollectionMapper.Tests.Common.Classes;

namespace RavenDB.CollectionMapper.Tests.Common
{
    public class MyMapperBuilder
    {
        public static MyMapper Build()
        {
            return (MyMapper)new MyMapper()
                .Map<Apple>("Fruits")
                .Map<Banana>("Fruits")
                .Map<Grape>("Fruits")
                .Map<Man>("People")
                .Map<Woman>("People");
        }
    }
}
