#if DEBUG
using NUnit.Framework;

namespace Logy.MwAgent.DotNetWikiBot.Smw.Tests
{
    [TestFixture]
    public class PersonPropertiesTests
    {
        [Test]
        public void MakeCondition()
        {
            Assert.AreEqual(
                "[[Birthday::+]] OR [[BirthdayText::+]] OR [[Deathday::+]] OR [[Sex::+]] OR [[Description::+]]|?Birthday|?BirthdayText|?Deathday|?Sex|?Description",
                PersonProperties.MakeCondition);
        }
    }
}
#endif
