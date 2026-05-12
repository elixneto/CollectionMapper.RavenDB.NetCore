namespace CollectionMapper.RavenDB.Models;

public class RavenDBCollectionDefinitionModel(Type collectionType, string collectionName)
{
    public Type CollectionType { get; } = collectionType;
    public string CollectionName { get; } = collectionName;
}