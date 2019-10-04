using System;

namespace CollectionMapper.RavenDB.NetCore.Exceptions
{
    public class CannotMapAbstractClassesException : Exception
    {
        public CannotMapAbstractClassesException(Type type)
            : base($"The class cannot be mapped. '{type.Name}' is an Abstract Class")
        { }
    }
}
