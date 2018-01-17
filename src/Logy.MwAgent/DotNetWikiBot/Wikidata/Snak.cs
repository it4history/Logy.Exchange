using Newtonsoft.Json.Linq;

namespace Logy.MwAgent.DotNetWikiBot.Wikidata
{
    public class Snak
    { 
        public string Snaktype { get; set; }
        public string Property { get; set; }

        /// <example>commonsMedia</example>
        public string Datatype { get; set; }
        public TypedValue Datavalue { get; set; }

        public object ValueTyped
        {
            get
            {
                if (Snaktype != "novalue")
                {
                    switch (Datavalue.Type ?? Datatype)
                    {
                        case "string":
                            return Datavalue.Value;
                        case "time":
                            return ((JObject)Datavalue.Value).ToObject<ValueTime>();
                        case "wikibase-entityid":
                        case "wikibase-item":
                            return ((JObject)Datavalue.Value).ToObject<ValueItem>();
                        case "globecoordinate":
                            return ((JObject)Datavalue.Value).ToObject<ValueCoor>();
                    }
                }
                return null;
            }
        }
    }
}