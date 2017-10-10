#if DEBUG
using System;
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
                "[[Birthday::+]] OR [[Deathday::+]] OR [[Description::+]]|?Birthday|?Deathday|?Description",
                PersonProperties.MakeCondition);
        }
    }
}
#endif
