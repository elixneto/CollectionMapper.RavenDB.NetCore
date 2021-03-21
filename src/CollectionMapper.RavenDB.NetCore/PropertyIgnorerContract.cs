using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CollectionMapper.RavenDB.NetCore
{
    public class PropertiesContract : DefaultContractResolver
    {
        private readonly List<string> _ignoredProperties = new List<string>();
        private bool _includeNonPublicProperties = false;

        public PropertiesContract() { }

        public void AddIgnoredProperties(string[] properties)
        {
            foreach (var p in properties)
                this._ignoredProperties.Add(p);
        }

        public void IncludeNonPublicProperties() => _includeNonPublicProperties = true;
        public void NotIncludeNonPublicProperties() => _includeNonPublicProperties = false;

        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            var members = new List<MemberInfo>();

            if (_includeNonPublicProperties)
                members.AddRange(objectType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
            else
                members.AddRange(objectType.GetProperties(BindingFlags.Instance | BindingFlags.Public));

            foreach (var prop in this._ignoredProperties)
                members.RemoveAll(x => x.Name == prop);

            return members;
        }

        public IReadOnlyList<string> GetIgnoredProperties() => _ignoredProperties;
    }
}
