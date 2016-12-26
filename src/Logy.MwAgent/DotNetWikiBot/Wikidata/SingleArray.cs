using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Logy.MwAgent.DotNetWikiBot.Wikidata
{
    [JsonArray]
    public class SingleArray<T> : JArray
    {
        public T A
        {
            get
            {
                return this.First().ToObject<T>();
            }
        }
    }
}