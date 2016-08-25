#if DEBUG

using System.Linq;

using DevExpress.Xpo;

using Logy.Entities;
using Logy.Entities.Documents;
using Logy.Entities.Model;
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
