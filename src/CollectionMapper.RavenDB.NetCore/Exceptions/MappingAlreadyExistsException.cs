using System;

namespace CollectionMapper.RavenDB.NetCore.Exceptions
{
    public class MappingAlreadyExistsException : Exception
    {
        public MappingAlreadyExistsException(Type type)
            : base($"Mapping already exists for: '{type.Name}'")
        { }
    }
}
