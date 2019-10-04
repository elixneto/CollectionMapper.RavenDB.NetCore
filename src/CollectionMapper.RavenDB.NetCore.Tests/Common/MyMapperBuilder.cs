using CollectionMapper.RavenDB.NetCore.Tests.Common.Classes;

namespace CollectionMapper.RavenDB.NetCore.Tests.Common
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
