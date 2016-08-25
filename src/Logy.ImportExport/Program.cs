using System;

using Logy.Entities.Products;

namespace Logy.ImportExport
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ProductsManager.UpdateUsed();
            ImportManager.ImportDescendantsOfAdamAndEve();
        }
    }
}
