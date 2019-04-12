using System;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Water;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean.Tests
{
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

        internal static void Cycle<T>(WaterMoving<T> data) where T: BasinBase
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