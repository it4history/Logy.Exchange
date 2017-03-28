using System.Collections.Generic;
using System.Linq;

namespace Logy.MwAgent.DotNetWikiBot.Wikidata
{
    public class Sitelinks 
    {
        public List<Sitelink> Translations { get; set; }

        public string GetTranslation(string lang)
        {
            return (from o in Translations
                where o.Site == lang + "wiki"
                select o.Title).SingleOrDefault();
        }
    }
}