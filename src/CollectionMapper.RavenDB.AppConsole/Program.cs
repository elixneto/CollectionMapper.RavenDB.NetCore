using CollectionMapper.RavenDB.AppConsole;
using CollectionMapper.RavenDB.AppConsole.Entities;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.Client.Json.Serialization.NewtonsoftJson;

var myCustomMapper = new MyCustomCollectionMapper();

myCustomMapper.Map<User>("USERX");
myCustomMapper.IncludeNonPublicProperties(false);

IDocumentStore store = new DocumentStore()
{
    Urls = ["http://localhost:8080"],
    Database = "DB_1",
    Conventions = {
        FindCollectionName = (type) => myCustomMapper.FindCollectionBy(type),
        Serialization = new NewtonsoftJsonSerializationConventions
        {
            JsonContractResolver  = myCustomMapper.GetPropertiesContract()
        }
    }
}.Initialize();

using (IDocumentSession session = store.OpenSession())
{
    var acc1 = new Account
    {
        Name = "BrazilBank-1"
    };
    session.Store(acc1);

    var acc2 = new Account
    {
        Name = "BrazilBank-2",
        Ammount = 350
    };
    session.Store(acc2);

    var user = new User
    {
        Id = new Random().Next().ToString(),
    };
    session.Store(user);

    session.SaveChanges();
}