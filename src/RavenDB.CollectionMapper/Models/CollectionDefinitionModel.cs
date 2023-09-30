using RavenDB.CollectionMapper.Exceptions;
using System;
using System.Reflection;

namespace RavenDB.CollectionMapper.Models
{
    public sealed class CollectionDefinitionModel
    {
        public readonly Type Type;
        public readonly string CollectionName;

        public CollectionDefinitionModel(Type type, string collectionName)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
            {
                throw new CollectionNameCannotBeEmptyException();
            }

            if (type.GetTypeInfo().IsAbstract)
            {
                throw new CannotMapAbstractClassesException(type);
            }

            this.Type = type;
            this.CollectionName = collectionName;
        }
    }
}
