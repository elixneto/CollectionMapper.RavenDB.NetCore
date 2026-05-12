using CollectionMapper.RavenDB;
using ConsoleApp.Entities;

namespace ConsoleApp;

public class MyCustomMapper : RavenDBCollectionMapper
{
    public MyCustomMapper()
    {
        Map<User>("USERX");
        Map<Strawberry>("FRUIT_OVERRIDEN");
    }
}