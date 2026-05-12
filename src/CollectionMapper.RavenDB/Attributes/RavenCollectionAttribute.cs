using CollectionMapper.RavenDB.Exceptions;

namespace CollectionMapper.RavenDB.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RavenCollectionAttribute : Attribute
{
    public string CollectionName { get; }

    public RavenCollectionAttribute(string collectionName)
    {
        if (string.IsNullOrWhiteSpace(collectionName))
        {
            throw new RavenDBEmptyCollectionNameException();
        }

        CollectionName = collectionName;
    }
}
