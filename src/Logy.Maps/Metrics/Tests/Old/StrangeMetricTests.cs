#if DEBUG
using System;
using System.Linq;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.World.Ocean;
using NUnit.Framework;

namespace Logy.Maps.Metrics.Tests.Old
{
    [TestFixture]
    public class StrangeMetricTests
    {
        [Test]
        public void Hto_SameRing()
        {
            var data = new OceanData(new HealpixManager(2)) { Spheric = true };
            data.Init();

            foreach (var aBasin in data.PixMan.Pixels)
            {
                Assert.AreEqual(0, aBasin.Hoq);
            }
            data.GradientAndHeightCrosses();

            for (var p = 43; p <= 50; p++)
            {
                var basin = data.PixMan.Pixels[p];
                Assert.AreEqual(137494, basin.Hto[0], 1);
                Assert.AreEqual(108624, basin.Hto[1], 1);
                TestNeighbors(basin);
            }
        }

        [Test]
        public void Hto()
        {
            var man = new HealpixManager(2);
            var data = new OceanData(man) { Spheric = true };
            data.Init();

            foreach (var aBasin in data.PixMan.Pixels)
            {
                Assert.AreEqual(0, aBasin.Hoq);
            }
            data.GradientAndHeightCrosses();

            var basin = data.PixMan.Pixels[(int)(man.Npix * .415)]; // 79 for k2, 20398 for k6
            Assert.AreEqual(96, basin.Hto[0] / 1000, 1);
            Assert.AreEqual(90, basin.Hto[1] / 1000, 1);
            TestNeighbors(basin);
            foreach (var aBasin in data.PixMan.Pixels)
            {
                if (aBasin != basin &&
                    !(from n in basin.Neighbors.Cast<Basin3>()
                        where n == aBasin
                        select n).Any())
                    Assert.AreEqual(0, aBasin.Hoq);
            }

            basin = data.PixMan.Pixels[63];
            Assert.AreEqual(109, basin.Hto[0] / 1000, 1);
            Assert.AreEqual(96, basin.Hto[1] / 1000, 1);
            TestNeighbors(basin);
            basin = data.PixMan.Pixels[47];
            Assert.AreEqual(137, basin.Hto[0] / 1000, 1);
            Assert.AreEqual(109, basin.Hto[1] / 1000, 1);
            TestNeighbors(basin);
            basin = data.PixMan.Pixels[31];
            Assert.AreEqual(156, basin.Hto[0] / 1000, 1);
            Assert.AreEqual(137, basin.Hto[1] / 1000, 1);
            TestNeighbors(basin);

            basin = data.PixMan.Pixels[17];
            Assert.AreEqual(145, basin.Hto[0] / 1000, 1);
            Assert.AreEqual(156, basin.Hto[1] / 1000, 1);
            TestNeighbors(basin);
            basin = data.PixMan.Pixels[7];
            Assert.AreEqual(139, basin.Hto[0] / 1000, 1);
            Assert.AreEqual(145, basin.Hto[1] / 1000, 1);
            TestNeighbors(basin);
        }

        private static void TestNeighbors(Basin3 basin)
        {
            // k6 2, k2 150, sphere .001
            var accuracy = .001;
            var ne = basin.Neighbors[Direction.Ne];
            var nw = basin.Neighbors[Direction.Nw];
            Assert.AreEqual(ne.Hto[1], basin.Hto[0], accuracy);
            if (ne.Ring == nw.Ring) Assert.AreEqual(ne.Hto[1], nw.Hto[1]);

            var sw = basin.Neighbors[Direction.Sw];
            var se = basin.Neighbors[Direction.Se];
            Assert.AreEqual(sw.Hto[0], basin.Hto[1], accuracy);
            Assert.AreEqual(sw.Hto[1], se.Hto[1]);

            if (basin.Hto[0] > basin.Hto[1])
                Assert.IsTrue(basin.Beta - ne.Beta > sw.Beta - basin.Beta);
            else
                Assert.IsFalse(basin.Beta - ne.Beta > sw.Beta - basin.Beta);

            Console.WriteLine(
                "Δβ {0:0.###}, ΔHto {1:0.###}",
                ////(basin.rOfEllipse + basin.Hto[0]) * Math.Sin((basin.Beta - ne.Beta).Value),
                ////(basin.rOfEllipse + basin.Hto[1]) * Math.Sin((se.Beta - basin.Beta).Value),
                (basin.Beta - ne.Beta) / (se.Beta - basin.Beta),
                basin.Hto[0] / basin.Hto[1]);
        }
    }
}
#endif