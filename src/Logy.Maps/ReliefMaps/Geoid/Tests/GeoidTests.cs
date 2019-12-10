using Logy.Maps.Projections.Healpix;
using MathNet.Spatial.Euclidean;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.Geoid.Tests
{
    [TestFixture]
    public class GeoidTests
    {
        [Test]
        public void NormalDatumUnderSolidAndOtherPolygons()
        {
            var data = new GeoidData(new HealpixManager(0))
            {
                WithRelief = true,
                Accuracy = 5
            };
            data.Init();

            var basins = data.PixMan.Pixels;
            basins[0].FillNewGeoid(data);
            Assert.AreEqual(SurfaceType.WorldOcean, basins[0].Polygon.SurfaceType);

            basins[1].FillNewGeoid(data);
            Assert.AreEqual(SurfaceType.Solid, basins[1].Polygon.SurfaceType);
            Assert.AreEqual(basins[0].RadiusGeoid, basins[1].RadiusGeoid);

            basins[2].FillNewGeoid(data);
            Assert.AreEqual(SurfaceType.WorldOcean, basins[2].Polygon.SurfaceType);
            Assert.AreEqual(basins[1].RadiusGeoid, basins[2].RadiusGeoid);

            basins[3].FillNewGeoid(data);
            Assert.AreEqual(SurfaceType.WorldOcean, basins[3].Polygon.SurfaceType);
            Assert.AreEqual(basins[2].Polygon, basins[3].Polygon);
            Assert.AreEqual(basins[0].Polygon, basins[3].Polygon);

            //Polygon
            //GeoidSurface
            //RadiusGeoid

            Geoid.Obtain(data);
            foreach (var basin in basins)
            {
                for (var from = 0; from < 4; from++)
                {
                    var ray = basin.MetricRays[from];
                    var edgePoint = basin.S_q.IntersectionWith(ray);
                    var geoidSurfaceForEdge = new Plane(basin.Normal.Value, edgePoint);

                    var expected = basin.S_q.ToString();
                    Assert.AreEqual(expected, geoidSurfaceForEdge.ToString().Substring(0, expected.Length));
                }

                Assert.AreEqual(
                    basin.RadiusGeoid,
                    basin.RadiusOfGeoid,
                    data.Water.Threshhold * (data.K < 3 ? 2.1 : 1));
            }
        }

        [Test]
        public void SphericDatumUnderSolidAndOtherPolygons()
        {
        }
    }
}