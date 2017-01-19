using Newtonsoft.Json;

namespace Logy.MwAgent.DotNetWikiBot.Wikidata
{
    public class ValueItem
    {
        [JsonProperty("entity-type")]
        public string EntityType { get; set; }

        public string Id { get; set; }

        [JsonProperty("numeric-id")]
        public int NumericId { get; set; }
    }
}