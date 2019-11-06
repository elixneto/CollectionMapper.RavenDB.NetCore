using CollectionMapper.RavenDB.NetCore.Exceptions;
using CollectionMapper.RavenDB.NetCore.Interfaces;
using CollectionMapper.RavenDB.NetCore.Models;
using Newtonsoft.Json.Serialization;
using Raven.Client.Documents.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CollectionMapper.RavenDB.NetCore
{
    public abstract class CollectionMapperRavenDB : ICollectionMapperRavenDB
    {
        private readonly IList<CollectionRavenDB> _collections = new List<CollectionRavenDB>();

        private readonly PropertyIgnorerContract _propertyIgnorer = new PropertyIgnorerContract();

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

        public CollectionMapperRavenDB Merge(ICollectionMapperRavenDB anotherCollectionMapper, bool mergeIgnorerContracts = true)
        {
            foreach (var coll in anotherCollectionMapper.GetCollections())
                this.Map(coll.CollectionName, coll.Type);

            if (mergeIgnorerContracts)
                this.IgnoreProperties(anotherCollectionMapper.GetIgnoredProperties());

            return this;
        }

        public bool IsMappedBy(Type type) => this._collections.Where(w => w.Type.Equals(type)).Any();

        public void IgnoreProperties(string[] properties) => _propertyIgnorer.AddProperties(properties);

        public IReadOnlyList<CollectionRavenDB> GetCollections() => _collections.ToList();

        public IContractResolver GetPropertyIgnorerContract() => _propertyIgnorer;

        public string[] GetIgnoredProperties() => _propertyIgnorer.GetIgnoredProperties().ToArray();

        private void Validate(Type type)
        {
            if (type.GetTypeInfo().IsAbstract)
                throw new CannotMapAbstractClassesException(type);

            if (IsMappedBy(type))
                throw new MappingAlreadyExistsException(type);
        }
    }
}
