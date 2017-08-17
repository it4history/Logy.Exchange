using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Logy.MwAgent.DotNetWikiBot.Wikidata
{
    public class SitelinksConverter : JsonConverter
    {
        public static IEnumerable<string> Languages { get; set; }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var translations = new List<Sitelink>();
            JToken token = JToken.Load(reader);
            foreach (var child in token.Children())
            {
                var sitelink = child.First.ToObject<Sitelink>();
                if (Languages != null)
                    foreach (var language in Languages)
                    {
                        if (sitelink.Site.StartsWith(language))
                        {
                            translations.Add(sitelink);
                        }
                    }
            }
            return new Sitelinks { Translations = translations };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }
}