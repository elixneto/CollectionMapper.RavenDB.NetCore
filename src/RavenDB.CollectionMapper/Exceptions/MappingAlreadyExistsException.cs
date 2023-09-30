using System;

namespace RavenDB.CollectionMapper.Exceptions
{
    public class MappingAlreadyExistsException : Exception
    {
        public MappingAlreadyExistsException(Type type)
            : base($"Mapping already exists for: '{type.Name}'")
        { }
    }
}
