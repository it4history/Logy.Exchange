using System.Collections;
using Logy.Entities.Import;
using Logy.Entities.Model;
using Logy.Entities.Products;
using Logy.ImportExport.Bible;
using Logy.MwAgent.DotNetWikiBot;

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
            IList pages = DescendantOfAdamAndEve.ParseDescendants(new Page(EventsDb.DescendantsOfAdamAndEve), true);
            ImportManager.Import(Wikipedia.Id, EventsDb.DescendantsOfAdamAndEve, typeof(DescendantOfAdamAndEveImporter), pages);
        }
    }
}
