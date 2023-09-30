using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RavenDB.CollectionMapper
{
    public class PropertiesContract : DefaultContractResolver
    {
        private readonly List<string> _ignoredProperties = new();
        private bool _includeNonPublicProperties = false;

        public PropertiesContract() { }

        public void AddIgnoredProperties(string[] properties)
        {
            if (!properties.Any())
            {
                return;
            }

            this._ignoredProperties.AddRange(properties.Distinct());
        }

        public void IncludeNonPublicProperties() => _includeNonPublicProperties = true;

        public void IgnoreNonPublicProperties() => _includeNonPublicProperties = false;

        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            var members = new List<MemberInfo>();

            if (_includeNonPublicProperties)
            {
                members.AddRange(objectType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
            }
            else
            {
                members.AddRange(objectType.GetProperties(BindingFlags.Instance | BindingFlags.Public));
            }

            foreach (var property in this._ignoredProperties)
            {
                members.RemoveAll(x => x.Name == property);
            }

            return members;
        }

        public IReadOnlyList<string> GetIgnoredProperties() => _ignoredProperties;
    }
}
