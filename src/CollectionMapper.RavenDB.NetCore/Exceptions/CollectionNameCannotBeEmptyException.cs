using System;

namespace CollectionMapper.RavenDB.NetCore.Exceptions
{
    public class CollectionNameCannotBeEmptyException : Exception
    {
        public CollectionNameCannotBeEmptyException()
            : base($"The name of the collection cannot be empty")
        { }
    }
}
