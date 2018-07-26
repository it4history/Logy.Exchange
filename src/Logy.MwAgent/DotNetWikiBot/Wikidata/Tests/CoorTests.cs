#if DEBUG

using NUnit.Framework;

namespace Logy.MwAgent.DotNetWikiBot.Wikidata.Tests
{
    [TestFixture]
    public class CoorTests
    {
        [Test]
        public void Constructor()
        {
            var coor = new Coor("70.1:52");
            Assert.AreEqual(70.1d, coor.X);
            Assert.AreEqual(52, coor.Y);
        }

        [Test]
        public void PhiTheta()
        {
            var coor = new Coor("-180:-90");
            Assert.AreEqual(-180d, coor.X);
            Assert.AreEqual(-90d, coor.Y);
            Assert.AreEqual(0, coor.Phi);
            Assert.AreEqual(0, coor.Theta);
        }
    }
}

#endif
