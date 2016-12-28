using Logy.Entities.Import;
using Logy.Entities.Model;
using Logy.Entities.Products;
using Logy.ImportExport.Bible;

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
            ImportManager.Import(Wikipedia.Id, EventsDb.DescendantsOfAdamAndEve, typeof(DescendantOfAdamAndEveImporter));
        }
    }
}
