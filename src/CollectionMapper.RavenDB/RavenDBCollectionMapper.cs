using CollectionMapper.RavenDB.Exceptions;
using CollectionMapper.RavenDB.Models;

namespace CollectionMapper.RavenDB;

public class RavenDBCollectionMapper
{
    private readonly List<RavenDBCollectionDefinitionModel> _mappedCollections = [];

    public IReadOnlyList<RavenDBCollectionDefinitionModel> MappedCollections => _mappedCollections;

    public RavenDBCollectionMapper Map<T>(string collectionName) where T : class
    {
        if (string.IsNullOrWhiteSpace(collectionName))
        {
            throw new RavenDBEmptyCollectionNameException();
        }

        var type = typeof(T);
        var existing = _mappedCollections.FindIndex(m => m.CollectionType == type);

        if (existing >= 0)
        {
            _mappedCollections[existing] = new RavenDBCollectionDefinitionModel(type, collectionName);
        }
        else
        {
            _mappedCollections.Add(new RavenDBCollectionDefinitionModel(type, collectionName));
        }

        return this;
    }
}
