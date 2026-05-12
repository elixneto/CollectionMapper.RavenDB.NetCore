![Sonar Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=elixneto_CollectionMapper.RavenDB.NetCore&metric=alert_status)
![Sonar Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=elixneto_CollectionMapper.RavenDB.NetCore&metric=vulnerabilities)
![Sonar Coverage](https://sonarcloud.io/api/project_badges/measure?project=elixneto_CollectionMapper.RavenDB.NetCore&metric=coverage)

# CollectionMapper.RavenDB.NetCore
<image width="160px" src="https://github.com/elixneto/CollectionMapper.RavenDB.NetCore/blob/master/src/CollectionMapper.RavenDB/logo.png" />

Map your C# types to custom RavenDB collection names — without touching `DocumentStore` configuration by hand for every entity.

## Installation

```
dotnet add package CollectionMapper.RavenDB.NetCore
```

## Quick start

Wire the mapper into your `DocumentStore` once, then register your types using any combination of the three approaches below.

```csharp
using CollectionMapper.RavenDB;
using Raven.Client.Documents;

// 1. Register your mappings and [decorators] (see options below)
RavenDBMapperConventions.RegisterByAssembly<MyEntity>();

// 2. Assign to the store conventions
IDocumentStore store = new DocumentStore
{
    Urls = ["http://localhost:8080"],
    Database = "MyDatabase",
    Conventions =
    {
        FindCollectionName = RavenDBMapperConventions.FindCollection,
    }
}.Initialize();
```

Any type that is not explicitly mapped falls back to RavenDB's default naming convention.

---

## Registration options

### 1. Fluent mapper

Use `RavenDBCollectionMapper` when you want to keep mapping configuration in one place, separate from your entity classes.

```csharp
var mapper = new RavenDBCollectionMapper();
mapper.Map<User>("MasterUsers")
      .Map<Order>("CustomOrders")
      .Map<Invoice>("Invs");

RavenDBMapperConventions.RegisterMapper(mapper);
```

Registering the same type twice keeps the last name (`Map` is last-write-wins).

#### Auto-discovered mapper class

Subclass `RavenDBCollectionMapper` and it will be picked up automatically by `RegisterByAssembly` / `RegisterByAssemblies` — no manual call to `RegisterMapper` needed.

```csharp
public class MyCollectionMapper : RavenDBCollectionMapper
{
    public MyCollectionMapper()
    {
        Map<User>("MasterUsers");
        Map<Order>("CustomOrders");
        Map<Invoice>("Invs");
    }
}
```

### 2. `[RavenCollection]` attribute

Decorate the class directly when you prefer co-location.

```csharp
using CollectionMapper.RavenDB.Attributes;

[RavenCollection("MyAccounts")]
public class Account
{
    public string Id { get; set; }
    public string Name { get; set; }
}

[RavenCollection("SuperUsers")]
public class User
{
    public string Id { get; set; }
    public string Name { get; set; }
}
```
Then scan the assembly that contains those classes:

```csharp
RavenDBMapperConventions.RegisterByAssembly<Account>();
```

### 3. `[RavenCollectionAssignedFrom<T>]` attribute

Assign a derived type to the same collection as its base type. Useful for polymorphic hierarchies stored in a single collection.

```csharp
using CollectionMapper.RavenDB.Attributes;

public class Fruit { }

[RavenCollectionAssignedFrom<Fruit>]
public class Banana : Fruit { }

[RavenCollectionAssignedFrom<Fruit>]
public class Strawberry : Fruit { }
```

`Banana` and `Strawberry` documents are stored in the `Fruits` collection, alongside `Fruit` documents.

---

## Assembly scanning

Scan one assembly (the one that contains `T`):

```csharp
RavenDBMapperConventions.RegisterByAssembly<MyEntity>();
```

Scan multiple assemblies at once:

```csharp
RavenDBMapperConventions.RegisterByAssemblies(
    typeof(MyEntity).Assembly,
    typeof(AnotherEntity).Assembly
);
```

Assembly scanning discovers both `[RavenCollection]` and `[RavenCollectionAssignedFrom<T>]` attributes, as well as any `RavenDBCollectionMapper` subclasses defined in those assemblies.

---

## Debug mode

Enable verbose logging to `Console` and `System.Diagnostics.Trace` to see every registration and every collection lookup:

```csharp
RavenDBMapperConventions.EnableDebugMode();
```

Example output:

```
[CollectionMapper.RavenDB] ::Register:: MyApp.Entities.Fruit → "Fruits" | source: [RavenCollection]
[CollectionMapper.RavenDB] ::Register:: MyApp.Entities.Banana → "Fruits" | source: [RavenCollectionAssignedFrom<Fruit>]
[CollectionMapper.RavenDB] ::FindCollection:: MyApp.Entities.Banana → "Fruits"
[CollectionMapper.RavenDB] ::FindCollection:: MyApp.Entities.Unmapped → "Unmappeds" (fallback)
```

Call `EnableDebugMode()` before registration calls to capture those log lines too.
