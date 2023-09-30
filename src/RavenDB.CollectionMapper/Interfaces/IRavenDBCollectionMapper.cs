using RavenDB.CollectionMapper.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace RavenDB.CollectionMapper.Interfaces
{
    public interface IRavenDBCollectionMapper
    {
        string FindCollectionBy(Type type);
        string FindCollectionBy<T>();
        IReadOnlyList<CollectionDefinitionModel> GetMappedCollections();

        RavenDBCollectionMapper Map<T>(string collectionName);
        RavenDBCollectionMapper Map(string collectionName, params Type[] types);
        RavenDBCollectionMapper Merge(IRavenDBCollectionMapper anotherCollectionMapper, bool mergeIgnorerContracts = true);
        bool HasMappingFor(Type type);
        bool HasMappingFor<T>();

        void IgnoreProperties(string[] properties);
        IContractResolver GetPropertiesContract();
        string[] GetIgnoredProperties();
        void IncludeNonPublicProperties(bool include = false);
    }
}
