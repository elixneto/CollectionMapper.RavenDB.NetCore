using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CollectionMapper.RavenDB.NetCore
{
    public class PropertyIgnorerContract : DefaultContractResolver
    {
        private readonly List<string> _ignoredProperties = new List<string>();

        public PropertyIgnorerContract() { }

        public void AddProperties(string[] properties)
        {
            foreach (var p in properties)
                this._ignoredProperties.Add(p);
        }


        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            var members = base.GetSerializableMembers(objectType);
            foreach (var prop in this._ignoredProperties)
                members.RemoveAll(x => x.Name == prop);

            return members;
        }

        public IReadOnlyList<string> GetIgnoredProperties() => _ignoredProperties;
    }
}
