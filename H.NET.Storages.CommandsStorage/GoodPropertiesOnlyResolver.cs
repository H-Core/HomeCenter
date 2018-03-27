using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace H.NET.Storages
{
    public class GoodPropertiesOnlyResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);

            return properties.Where(p => p.Writable && !typeof(Delegate).IsAssignableFrom(p.PropertyType)).ToList();
        }
    }
}