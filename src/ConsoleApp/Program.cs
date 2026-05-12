using CollectionMapper.RavenDB;
using ConsoleApp.Entities;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

var myCustomMapper = new RavenDBCollectionMapper();
myCustomMapper.Map<User>("USER_CUSTOM");
myCustomMapper.Map<Strawberry>("TEMP_STRAWBERRY_CUSTOM");

RavenDBMapperConventions.EnableDebugMode();
RavenDBMapperConventions.RegisterMapper(myCustomMapper);
RavenDBMapperConventions.RegisterByAssembly<Program>();

IDocumentStore store = new DocumentStore()
{
    Urls = ["http://localhost:8080"],
    Database = "DB_1",
    Conventions = {
        FindCollectionName = RavenDBMapperConventions.FindCollection,
    }
}.Initialize();

using (IDocumentSession session = store.OpenSession())
{
    session.Store(new Account
    {
        Name = "BrazilBank-1"
    });
    
    session.Store(new Account
    {
        Name = "BrazilBank-2",
        Ammount = 350
    });
    
    session.Store(new User());
    
    session.Store(new Unmapped());
    
    session.Store(new Fruit());
    session.Store(new Banana());
    session.Store(new Strawberry());
    
    session.SaveChanges();
}