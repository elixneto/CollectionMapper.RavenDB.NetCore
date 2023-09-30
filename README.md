<image width="150px" src="https://github.com/elixneto/CollectionMapper.RavenDB.NetCore/blob/master/src/CollectionMapper.RavenDB.NetCore/logo.png" />

# CollectionMapper.RavenDB.NetCore
An easy way to map your domain/entities classes to RavenDB collections for .NET applications

## Wiki
Full Documentation: <br>
https://github.com/elixneto/CollectionMapper.RavenDB.NetCore/wiki

## Easy to use
```csharp
public class MyCustomMapper : RavenDBCollectionMapper
{
    public MyCustomMapper()
    {
        Map<Account>("MyUniqueAccounts");
        Map<Bank>("SuperSpecialBanks");
        Map<User>("USER");
        Map("Fruits", typeof(Apple), typeof(Banana), typeof(Strawberry));
        ...
        ...
    }
}

public class RavenDBDocumentStoreHolderExample
{
    private readonly MyCustomMapper myCustomMapper = new MyCustomMapper();

    private static Lazy<IDocumentStore> store = new Lazy<IDocumentStore>(CreateStore);
    public static IDocumentStore Store => store.Value;
    private static IDocumentStore CreateStore()
    {
        IDocumentStore store = new DocumentStore
        {
                Urls = new[] { "http://your_RavenDB_cluster_node" },
                Database = "your_RavenDB_database_name",
                
                /* Here you set the collection mapper */
                Conventions = {
                    FindCollectionName = (type) => myCustomMapper.FindCollectionBy(type)
                }

        }.Initialize();

        return store;
    }
}
```
