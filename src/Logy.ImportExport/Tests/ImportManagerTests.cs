#if DEBUG

using System.Linq;

using DevExpress.Xpo;

using Logy.Entities;
using Logy.Entities.Documents;
using Logy.Entities.Model;
using Logy.Entities.Persons;
using Logy.Entities.Products;

using NUnit.Framework;

namespace Logy.ImportExport.Tests
{
    [TestFixture]
    public class ImportManagerTests 
    {
        [Test]
        public void ImportPerson()
        {
            Import(EntityType.Person);
        }

        [Test]
        public void ImportEventTypes()
        {
            Import(EntityType.EventType);
        }

        [Test]
        public void ImportDescendantOfAdamAndEve()
        {
            ImportManager.ImportDescendantsOfAdamAndEve();
        }

       /* [Test]
        public void ImportWikidata()
        {
            var man = new ImportManager(new Parameter { Admin = true });
            foreach (var pname in (from o in new XPQuery<PName>(man.XpoSession)
                where o.WikidataItemId == null
                select o))
            {

            }
        }*/

        private void Import(EntityType type)
        {
            var man = new ImportManager(new Parameter { Admin = true });
            var template = (from o in new XPQuery<ImportTemplate>(man.XpoSession)
                            where o.Url == ImportTemplate.UrlFromEntityType(type)
                            select o).Single();

            man.Import(man.XpoSession.GetObjectByKey<Site>(LogyEventsDb.LogyWikiId), template);
        }
    }
}
#endif
