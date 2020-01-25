using CollectionMapper.RavenDB.NetCore.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace CollectionMapper.RavenDB.NetCore.Interfaces
{
    public interface ICollectionMapperRavenDB
    {
        bool IsMappedBy(Type type);
        string FindCollectionBy(Type type);
        CollectionMapperRavenDB Map<T>(string collectionName);
        CollectionMapperRavenDB Map(string collectionName, params Type[] types);
        CollectionMapperRavenDB Merge(ICollectionMapperRavenDB anotherCollectionMapper, bool mergeIgnorerContracts = true);

        IReadOnlyList<CollectionRavenDB> GetCollections();

        void IgnoreProperties(string[] properties);
        string[] GetIgnoredProperties();
        void IncludeNonPublicProperties();
        void NotIncludeNonPublicProperties();

        IContractResolver GetPropertiesContract();
    }
}
