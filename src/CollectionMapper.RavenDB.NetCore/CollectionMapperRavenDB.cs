using CollectionMapper.RavenDB.NetCore.Exceptions;
using Raven.Client.Documents.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CollectionMapper.RavenDB.NetCore
{
    public abstract class CollectionMapperRavenDB
    {
        public IReadOnlyList<CollectionRavenDB> Collections => _collections.ToList();
        private readonly IList<CollectionRavenDB> _collections = new List<CollectionRavenDB>();

        public string FindCollectionBy(Type type) => this._collections.Where(w => w.Type == type).SingleOrDefault()?.CollectionName ?? DocumentConventions.DefaultGetCollectionName(type);

        public CollectionMapperRavenDB Map<T>(string collectionName)
        {
            var type = typeof(T);
            Validate(type);

            this._collections.Add(new CollectionRavenDB(type, collectionName));

            return this;
        }
        public CollectionMapperRavenDB Map(string collectionName, params Type[] types)
        {
            foreach (var type in types)
            {
                Validate(type);
                this._collections.Add(new CollectionRavenDB(type, collectionName));
            }

            return this;
        }

        public CollectionMapperRavenDB Merge(CollectionMapperRavenDB anotherCollectionMapper)
        {
            foreach (var coll in anotherCollectionMapper._collections)
                this.Map(coll.CollectionName, coll.Type);

            return this;
        }

        public bool IsMappedBy(Type type) => this._collections.Where(w => w.Type.Equals(type)).Any();

        private void Validate(Type type)
        {
            if (type.GetTypeInfo().IsAbstract)
                throw new CannotMapAbstractClassesException(type);

            if (IsMappedBy(type))
                throw new MappingAlreadyExistsException(type);
        }
    }
}
