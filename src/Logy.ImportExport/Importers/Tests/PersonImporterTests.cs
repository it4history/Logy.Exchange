#if DEBUG
using Logy.Entities.Persons;
using NUnit.Framework;

namespace Logy.ImportExport.Importers.Tests
{
    [TestFixture]
    public class PersonImporterTests
    {
        [Test]
        public void GetPersonTypeByCategories()
        {
            Assert.AreEqual(
                PersonType.Human,
                PersonImporter.GetPersonTypeByCategories(new[] { "Human", "Category:Organization" }));
            Assert.AreEqual(
                PersonType.Organization,
                PersonImporter.GetPersonTypeByCategories(new[] { "Category:Person", "Category:Organization" }));
            Assert.AreEqual(
                PersonType.Dynasty,
                PersonImporter.GetPersonTypeByCategories(new[] { "Person", "Dynasty", "Human" }));
            Assert.AreEqual(
                PersonType.Person,
                PersonImporter.GetPersonTypeByCategories(null));
        }
    }
}
#endif