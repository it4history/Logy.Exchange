using System;
using System.Collections.Generic;
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
                    var token = property.Value.First;
                    if (token != null)
                    {
                        var value = token.ToObject<PropertyValue>();
                        value.Name = property.Key;
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
                    if (property.Key == name && property.Value.First != null)
                        return property.Value.First.ToObject<PropertyValue>();
                }
                return null;
            }
        }

        public T Get<T>()
        {
            T o = Activator.CreateInstance<T>();
            foreach (var prop in typeof(T).GetProperties())
            {
                var value = this[prop.Name];
                if (value != null)
                {
                    if (prop.PropertyType == typeof(DateTime?))
                        prop.SetValue(o, value.DateTime, null);
                }
            }
            return o;
        }
    }
}