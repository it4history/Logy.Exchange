using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Logy.MwAgent.DotNetWikiBot.Wikidata
{
    public class Reference
    {
        public string Hash { get; set; }

        public JObject Snaks { get; set; }

        public List<Snak> SnaksList
        {
            get
            {
                var list = new List<Snak>();
                foreach (var child in Snaks.Children())
                {
                    list.Add(child.First.ToObject<SingleArray<Snak>>().A);
                }
                return list;
            }
        }

        [JsonProperty("snaks-order")]
        public string[] SnaksOrder { get; set; }
    }
}