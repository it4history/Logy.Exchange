using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Logy.MwAgent.DotNetWikiBot.Wikidata
{
    public class WikidataParser
    {
        public static Result ParseWikidataItem(int? itemId)
        {
            string jsonSrc = Site.PostDataAndGetResult("http://www.wikidata.org/wiki/Special:EntityData/Q" + itemId + ".json", null, null); // raises "404: Not found" if not found
            return ParseWikidataItem(jsonSrc);
        }

        public static Result ParseWikidataItem(string json)
        {
            json = json.Replace("\"claims\":[]", "\"claims\":{}");
            var result = JObject.Parse(json)["entities"].Single().Single().ToObject<Result>();
            Console.WriteLine(
                Bot.Msg("Wikidata item {0} associated was parsed successfully."),
                result.Title);
            return result;
        }

        /// <summary>For pages of Wikimedia foundation projects this function returns
        /// interlanguage links located on <see href="https://wikidata.org">
        /// Wikidata.org</see>.</summary>
        /// <returns>Returns the List&lt;string&gt; object.</returns>
        public static List<string> GetWikidataLinks(Page page)
        {
            var src = page.Site.GetWebPage(page.Site.IndexPath + "?title=" + Bot.UrlEncode(page.Title));
            var list = new List<string>();
            if (!src.Contains("<li class=\"interlanguage-link "))
                return list;
            src = "<ul>" + Bot.GetSubstring(src, "<li class=\"interlanguage-link ", "</ul>");
            MatchCollection matches = Regex.Matches(src, "interlanguage-link interwiki-([^\" ]+)");
            list.AddRange(from Match m in matches select m.Groups[1].Value);
            return list;
        }

        /// <summary>For pages that have associated items on <see href="https://wikidata.org">
        /// Wikidata.org</see> this function returns
        /// If page is not associated with a Wikidata item null is returned.
        /// Gets translations for SitelinksConverter.Languages but maybe GetWikidataLinks() is good for translations too</summary>
        public static Result GetWikidataItem(Page page)
        {
            string src = page.Site.GetWebPage(page.Site.IndexPath + "?title=" + Bot.UrlEncode(page.Title));
            Match m = Regex.Match(src, "href=\"//www\\.wikidata\\.org/wiki/(Q\\d+)");
            if (!m.Success) // fallback
                m = Regex.Match(src, "\"wgWikibaseItemId\"\\:\"(Q\\d+)\"");
            if (!m.Success)
            {
                Console.WriteLine(Bot.Msg("No Wikidata item is associated with page \"{0}\"."), page.Title);
                return null;
            }

            return new Result { Title = m.Groups[1].Value };
        }
    }
}
