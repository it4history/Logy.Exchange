using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Logy.MwAgent.DotNetWikiBot.Wikidata
{
    public class SitelinksConverter : JsonConverter
    {
        private static List<string> _languages = new List<string> { "en", "uk", "ru" };

        public static List<string> Languages
        {
            get { return _languages; }
            set { _languages = value; }
        }

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
                foreach (var language in _languages)
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