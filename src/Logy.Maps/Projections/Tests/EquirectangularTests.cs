#if DEBUG
using Logy.Maps.Projections.Healpix;
using Logy.MwAgent.DotNetWikiBot.Wikidata;
using NUnit.Framework;

namespace Logy.Maps.Projections.Tests
{
    [TestFixture]
    public class EquirectangularTests
    {
        [Test]
        public void Offset()
        {
            var e = new Equirectangular(5);
            var coor = new Coor { X = 0, Y = 0 };
            var allRecords = 2160 * 4320;
            Assert.AreEqual(2160, e.Offset(coor).X);
            Assert.AreEqual(1080, e.Offset(coor).Y);

            e = new Equirectangular(new HealpixManager(5));
            Assert.AreEqual(64, e.Offset(coor).X);//or 64?
            Assert.AreEqual(32, e.Offset(coor).Y);
            coor = new Coor { X = 180, Y = -90 };
            Assert.AreEqual(127, e.Offset(coor).X);
            Assert.AreEqual(64, e.Offset(coor).Y);//or 64?

            coor = new Coor { X = -180, Y = -90 };
            Assert.AreEqual(0, e.Offset(coor).X);
        }

        [Test]
        public void OffsetEarth2014()
        {
            var coor = new HealCoor { Latitude = -89.9917, Longitude = -179.9917 };
            
            Assert.AreEqual(0, coor.Offset()); 

            var allRecords = 10800 * 21600; // 233280000;
            coor = new HealCoor { Latitude = 0, Longitude = 0 };

            // not accurate transforming
            Assert.AreEqual(allRecords / 2d, coor.Offset(), 11000);

            // not accurate transforming
            coor = new HealCoor { Latitude = 89.9917, Longitude = 179.9917 };
            Assert.AreEqual(allRecords - 1, coor.Offset());
        }
    }
}
#endif
