using System;

namespace CollectionMapper.RavenDB.NetCore.Exceptions
{
    public class MappingAlreadyExistsException : Exception
    {
        public MappingAlreadyExistsException() : base("Mapping already exists") { }
    }
}
