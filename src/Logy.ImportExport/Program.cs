using Logy.Entities.Import;
using Logy.Entities.Products;
using Logy.ImportExport.Bible;
using Skills.Xpo;

namespace Logy.ImportExport
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // ProductsManager.StartApi(false);
            ProductsManager.UpdateUsed();
            new ImportManager(DalManager.NewSession).PrepareTemplatesAndSites();

            ImportManager.Import(typeof(DescendantOfAdamAndEveImporter));
        }
    }
}
