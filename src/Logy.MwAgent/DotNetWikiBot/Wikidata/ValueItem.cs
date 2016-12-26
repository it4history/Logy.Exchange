using Newtonsoft.Json;

namespace Logy.MwAgent.DotNetWikiBot.Wikidata
{
    public class ValueItem
    {
        public const int Male = 6581097;
        public const int Female = 6581072;

        [JsonProperty("entity-type")]
        public string EntityType { get; set; }

        public string Id { get; set; }

        [JsonProperty("numeric-id")]
        public int NumericId { get; set; }
    }
}