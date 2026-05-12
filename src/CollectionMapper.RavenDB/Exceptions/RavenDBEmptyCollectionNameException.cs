namespace CollectionMapper.RavenDB.Exceptions;

public class RavenDBEmptyCollectionNameException : Exception
{
    public RavenDBEmptyCollectionNameException() : base("Collection name must not be empty.") { }
}