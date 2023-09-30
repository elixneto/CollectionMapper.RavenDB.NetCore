using Newtonsoft.Json.Serialization;
using CollectionMapper.RavenDB.NetCore.Exceptions;
using CollectionMapper.RavenDB.NetCore.Interfaces;
using CollectionMapper.RavenDB.NetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CollectionMapper.RavenDB.NetCore
{
    public abstract class RavenDBCollectionMapper : IRavenDBCollectionMapper
    {
        private readonly List<CollectionDefinitionModel> _mappedCollections = new List<CollectionDefinitionModel>();
        private readonly PropertiesContract _contractProperties = new PropertiesContract();

        public string FindCollectionBy(Type type) => this._mappedCollections.SingleOrDefault(w => w.Type == type)?.CollectionName;

        public string FindCollectionBy<T>() => this.FindCollectionBy(typeof(T));

        public IReadOnlyList<CollectionDefinitionModel> GetMappedCollections() => _mappedCollections.AsReadOnly();

        public RavenDBCollectionMapper Map<T>(string collectionName) => this.PrivateMap(collectionName, typeof(T));
        
        public RavenDBCollectionMapper Map(string collectionName, params Type[] types)
        {
            foreach (var type in types)
            {
                this.PrivateMap(collectionName, type);
            }

            return this;
        }

        public RavenDBCollectionMapper Merge(IRavenDBCollectionMapper anotherCollectionMapper, bool mergeIgnorerContracts = true)
        {
            foreach (var coll in anotherCollectionMapper.GetMappedCollections())
            {
                this.PrivateMap(coll.CollectionName, coll.Type);
            }

            if (mergeIgnorerContracts)
            {
                this.IgnoreProperties(anotherCollectionMapper.GetIgnoredProperties());
            }

            return this;
        }

        public bool HasMappingFor(Type type) => this._mappedCollections.Exists(w => w.Type.Equals(type));

        public bool HasMappingFor<T>() => this._mappedCollections.Exists(w => w.Type.Equals(typeof(T)));

        public void IgnoreProperties(string[] properties) => _contractProperties.AddIgnoredProperties(properties);

        public IContractResolver GetPropertiesContract() => _contractProperties;

        public string[] GetIgnoredProperties() => _contractProperties.GetIgnoredProperties().ToArray();

        public void IncludeNonPublicProperties(bool include = false)
        {
            if (include)
            {
                _contractProperties.IncludeNonPublicProperties();
                return;
            }

            _contractProperties.IgnoreNonPublicProperties();
        }

        private RavenDBCollectionMapper PrivateMap(string collectionName, Type type)
        {
            Validate(type);

            var collectionDefinitionModel = new CollectionDefinitionModel(type, collectionName);
            _mappedCollections.Add(collectionDefinitionModel);

            return this;
        }

        private void Validate(Type type)
        {
            if (HasMappingFor(type))
            {
                throw new MappingAlreadyExistsException(type);
            }
        }
    }
}
