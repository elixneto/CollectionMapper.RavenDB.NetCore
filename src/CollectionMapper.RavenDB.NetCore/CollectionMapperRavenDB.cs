using CollectionMapper.RavenDB.NetCore.Exceptions;
using Raven.Client.Documents.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CollectionMapper.RavenDB.NetCore
{
    public abstract class CollectionMapperRavenDB
    {
        private readonly IList<CollectionRavenDB> Collections = new List<CollectionRavenDB>();

        public CollectionMapperRavenDB() { }

        public string FindCollection(Type type) => this.Collections.Where(w => w.Type == type).SingleOrDefault()?.CollectionName ?? DocumentConventions.DefaultGetCollectionName(type);

        protected CollectionMapperRavenDB Map<T>(string collectionName)
        {
            var type = typeof(T);

            if (IsMappedByType(type))
                throw new MappingAlreadyExistsException();

            this.Collections.Add(new CollectionRavenDB(type, collectionName));

            return this;
        }

        protected CollectionMapperRavenDB Merge(CollectionMapperRavenDB anotherCollectionMapper)
        {
            foreach (var collection in anotherCollectionMapper.Collections)
                this.Collections.Add(collection);

            return this;
        }

        private bool IsMappedByType(Type type) => this.Collections.Where(w => w.Type == type).Any();
    }
}
