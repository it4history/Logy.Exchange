using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.Geoid.Tests
{
    [TestFixture]
    public class GeoidTests
    {
        [Test]
        public void NormalDatumUnderSolidAndOtherPolygons()
        {
            var data = new BasinDataAbstract<BasinOfGeoid>(new HealpixManager(2))
                { WithRelief = true };
            data.Init();

            Geoid.Obtain(data);
            foreach (var basin in data.PixMan.Pixels)
                if (basin.Polygon.SurfaceType == SurfaceType.Solid)
                    Assert.AreEqual(basin.S_geiod.ToString(), basin.GeoidSurfaceForSolid.ToString());
        }

        [Test]
        public void SphericDatumUnderSolidAndOtherPolygons()
        {
        }
    }
}