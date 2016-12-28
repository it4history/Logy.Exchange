using System.Collections;
using Logy.Entities.Import;
using Logy.Entities.Model;
using Logy.Entities.Products;
using Logy.ImportExport.Bible;
using Logy.MwAgent.DotNetWikiBot;
using Site = Logy.MwAgent.DotNetWikiBot.Site;

namespace Logy.ImportExport
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ProductsManager.UpdateUsed();
            ImportDescendantsOfAdamAndEve();
        }

        public static void ImportDescendantsOfAdamAndEve()
        {
            var page = new Page(new Site(Site.WikipediaBaseUrl), EventsDb.DescendantsOfAdamAndEve);
            IList pages = DescendantOfAdamAndEve.ParseDescendants(page, true);
            ImportManager.Import(Wikipedia.Id, EventsDb.DescendantsOfAdamAndEve, typeof(DescendantOfAdamAndEveImporter), pages);
        }
    }
}
