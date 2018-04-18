#if DEBUG

using NUnit.Framework;

namespace Logy.MwAgent.DotNetWikiBot.Wikidata.Tests
{
    [TestFixture]
    public class ValueCoorTests
    {
        [Test]
        public void Constructor()
        {
            var coor = new ValueCoor("70.1-52");
            Assert.AreEqual(70.1d, coor.X);
            Assert.AreEqual(52, coor.Y);
        }
    }
}

#endif
