using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
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
            var data = new BasinDataAbstract<BasinOfGeoid>(new HealpixManager(5))
            {
                WithRelief = true,
                Accuracy = 1
            };
            data.Init();

            Geoid.Obtain(data);
            foreach (var basin in data.PixMan.Pixels)
            {
                for (var from = 0; from < 4; from++)
                {
                    var ray = basin.MeanEdges[from];
                    var edgePoint = basin.S_q.IntersectionWith(ray);
                    var geoidSurfaceForEdge = new Plane(basin.Normal.Value, edgePoint);

                    var expected = basin.S_q.ToString();
                    Assert.AreEqual(expected, geoidSurfaceForEdge.ToString().Substring(0, expected.Length));
                }

                if (basin.Polygon.SurfaceType == SurfaceType.Solid)
                  Assert.AreEqual(
                      basin.GeoidRadius, 
                      basin.RadiusOfEllipse, 
                      data.Water.Threshhold * (data.K < 3 ? 2.1 : 1));
            }
        }

        [Test]
        public void SphericDatumUnderSolidAndOtherPolygons()
        {
        }
    }
}