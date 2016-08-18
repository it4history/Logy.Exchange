#if DEBUG
using NUnit.Framework;

namespace Logy.MwAgent.DotNetWikiBot.Tests
{
    [TestFixture]
    public class BotTests 
    {
        [Test]
        public void PageList()
        {
            var site = new Site("http://logy.gq/lw");
            var pl = new PageList(site);
            pl.FillFromCategoryTree("Category:Organization");

            /*            // Fill it with 100 pages, where "nuclear disintegration" is mentioned
            pl.FillFromGoogleSearchResults("nuclear disintegration", 100);
            // Load texts and metadata of all found pages from live wiki
            pl.LoadWithMetadata();

            // Now suppose, that we must correct some typical mistake in all our pages
            foreach (Page i in pl)
                // In each page we will replace one phrase with another
                i.text = i.text.Replace("fusion products", "fission products");
            // Finally we'll save all changed pages to wiki with 5 seconds interval            
            pl.SaveSmoothly(5, "comment: mistake autocorrection", true);

            // Now clear our PageList so we could re-use it
            pl.Clear();*/
            
            // Fill it with all articles in "Astronomy" category and it's subcategories
        }
    }
}
#endif
