# CollectionMapper.RavenDB.NetCore
An easy way to map your domain/entities classes to RavenDB collections for .Net Core applications

## Wiki
Full Documentation: <br>
https://github.com/elixneto/CollectionMapper.RavenDB.NetCore/wiki

## Easy to use
```csharp
public class MyMapper : CollectionMapperRavenDB
{
    public MyMapper()
    {
        Map<Account>("MyUniqueAccounts");
        Map<Bank>("SuperSpecialBanks");
        Map<User>("xUsersx");
        ...
        ...
    }
}

public class RavenDBContextExample
{
    private readonly MyMapper myMapper = new MyMapper();

    public RavenDBContextExample()
    {
        using(IDocumentStore store = new DocumentStore
        {
                Urls = new[] { "your_RavenDB_server_URL" },
                Database = "your_RavenDB_database_name",
                
                /* Here you set the collection mapper */
                Conventions = {
                    FindCollectionName = (type) => myMapper.FindCollectionBy(type)
                }

        }.Initialize())
    }
}
```
