using System.Collections.Generic;
using Logy.MwAgent.DotNetWikiBot.Exceptions;
using Newtonsoft.Json.Linq;

namespace Logy.MwAgent.DotNetWikiBot.Smw
{
    public class SemanticWikiParser
    {
        public static Dictionary<string, PageProperties> Ask(Site site, string query)
        {
            return Parse(Get(site, query));
        }

        internal static string Get(Site site, string query)
        {
            string respStr = site.PostDataAndGetResult(
                site.ApiPath + "?action=ask&format=json&query=" + Bot.UrlEncode(query),
                null);
            if (respStr.Contains("<error"))
                throw new WikiBotException(
                    string.Format(Bot.Msg("Failed to ask \"{0}\"."), query));
            return respStr;
        }

        internal static Dictionary<string, PageProperties> Parse(string json)
        {
            var dic = new Dictionary<string, PageProperties>();
            foreach (JProperty result in JObject.Parse(json)["query"]["results"])
            {
                dic.Add(result.Name, result.Value.ToObject<PageProperties>());
            }
            return dic;
        }
    }
}