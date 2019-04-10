using System;
using System.Linq;
using Logy.Maps.Projections.Healpix;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean.Tests
{
    [TestFixture]
    public class BasinDataDotProductTests
    {
        [Test]
        public void Hto_SameRing()
        {
            var data = new BasinData(new HealpixManager(2), false, true);

            foreach (var aBasin in data.PixMan.Pixels)
            {
                Assert.AreEqual(0, aBasin.hOQ);
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
            var data = new BasinData(man, false, true);

            foreach (var aBasin in data.PixMan.Pixels)
            {
                Assert.AreEqual(0, aBasin.hOQ);
            }
            data.GradientAndHeightCrosses();

            var basin = data.PixMan.Pixels[(int)(man.Npix * .415)]; // 79 for k2, 20398 for k6
            Assert.AreEqual(96, basin.Hto[0] / 1000, 1);
            Assert.AreEqual(90, basin.Hto[1] / 1000, 1);
            TestNeighbors(basin);
            foreach (var aBasin in data.PixMan.Pixels)
            {
                if (aBasin != basin &&
                    !(from n in basin.Neibors.Cast<Basin3>()
                        where n == aBasin
                        select n).Any())
                    Assert.AreEqual(0, aBasin.hOQ);
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
            var ne = basin.Neibors[Direction.Ne];
            var nw = basin.Neibors[Direction.Nw];
            Assert.AreEqual(ne.Hto[1], basin.Hto[0], accuracy);
            if (ne.Ring == nw.Ring) Assert.AreEqual(ne.Hto[1], nw.Hto[1]);

            var sw = basin.Neibors[Direction.Sw];
            var se = basin.Neibors[Direction.Se];
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

    [TestFixture]
    public class BasinDataTests
    {
        [Test]
        public void HighBasin_31()
        {
            var data = new BasinData(new HealpixManager(2), false, true);
            var basin3 = data.PixMan.Pixels[31];
            foreach (var basin in data.PixMan.Pixels)
            {
                for (var i = 0; i < 4; i++)
                    Assert.IsFalse(basin.Volumes[i]);
            }

            basin3.hOQ = 500;

            //            data.Cycle();
            Cycle(data);
            Cycle(data);
            Cycle(data);
            Cycle(data);
        }

        [Test]
        public void HighBasin_62_126_ellipse()
        {
            var data = new BasinData(new HealpixManager(2), false, false);
            data.PixMan.Pixels[45].hOQ = 500;
            Cycle(data);
            //Assert.AreEqual(99, data.PixMan.Pixels[33].hOQ, 1);
            Assert.AreEqual(50, //// 99 for SignedDistanceTo, 50 for MeanEdges
                data.PixMan.Pixels[62].hOQ, 1);
            Assert.AreEqual(298, //// 203 for SignedDistanceTo, 298 for MeanEdges 
                data.PixMan.Pixels[45].hOQ, 1);
//            Assert.AreEqual(99, data.PixMan.Pixels[77].hOQ, 1);
            Cycle(data);

            data = new BasinData(new HealpixManager(2), false, false);
            data.PixMan.Pixels[126].hOQ = 500;
            Cycle(data);
            Assert.AreEqual(50, data.PixMan.Pixels[110].hOQ, 1);
            Assert.AreEqual(50, data.PixMan.Pixels[142].hOQ, 1);
        }

        private static void Cycle(BasinData data)
        {
            Console.WriteLine("---------");
            data.Cycle();
            foreach (var basin in data.PixMan.Pixels)
            {
                if (basin.hOQ != 0)
                    Console.WriteLine("{0} {1:#.#}", basin.P, basin.hOQ);
            }
        }
    }
}