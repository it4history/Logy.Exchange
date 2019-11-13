using Logy.Entities.Import;
using Logy.Entities.Products;
using Logy.ImportExport.Bible;

namespace Logy.ImportExport
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ProductsManager.StartApi(false);

            ImportManager.Import(typeof(DescendantOfAdamAndEveImporter));
        }
    }
}
