using System;

namespace CollectionMapper.RavenDB.NetCore
{
    internal class CollectionRavenDB
    {
        public readonly Type Type;
        public readonly string CollectionName;

        public CollectionRavenDB(Type type, string collectionName)
        {
            this.Type = type;
            this.CollectionName = collectionName;
        }
    }
}
