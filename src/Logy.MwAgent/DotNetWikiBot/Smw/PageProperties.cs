using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Logy.MwAgent.DotNetWikiBot.Smw
{
    public class PageProperties
    {
        public string Fullurl { get; set; }
        public JObject Printouts { get; set; }

        public List<PropertyValue> Properties
        {
            get
            {
                var list = new List<PropertyValue>();
                foreach (var property in Printouts)
                {
                    var value = GetPropertyValue(property.Value.First, property);
                    if (value != null)
                    {
                        list.Add(value);
                    }
                }
                return list;
            }
        }

        public PropertyValue this[string name]
        {
            get
            {
                foreach (var property in Printouts)
                {
                    if (property.Key == name)
                    {
                        return GetPropertyValue(property.Value.First, property);
                    }
                }
                return null;
            }
        }

        public T Get<T>()
        {
            T o = Activator.CreateInstance<T>();
            foreach (var prop in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var value = this[prop.Name];
                if (value != null)
                {
                    if (prop.PropertyType == typeof(DateTime?))
                        prop.SetValue(o, value.DateTime, null);
                    else
                        prop.SetValue(o, value.Raw, null);
                }
            }
            return o;
        }

        private static PropertyValue GetPropertyValue(JToken token, KeyValuePair<string, JToken> property)
        {
            if (token != null)
            {
                var value = token.Type == JTokenType.String
                    ? new PropertyValue { Raw = property.Value.First.Value<string>() }
                    : token.ToObject<PropertyValue>();
                value.Name = property.Key;
                return value;
            }
            return null;
        }
    }
}