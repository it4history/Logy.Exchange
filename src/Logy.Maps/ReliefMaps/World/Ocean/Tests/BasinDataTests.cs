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
            var data = new BasinData(new HealpixManager(2));
            data.OnInit();
            var basin3 = data.PixMan.Pixels[31];
            foreach (var basin in data.PixMan.Pixels)
            {
                for (var i = 0; i < 4; i++)
                    Assert.IsFalse(basin.Volumes[i]);
            }

            basin3.HeightOQ = 500;

            // data.Frame();
            DoFrame(data);
            DoFrame(data);
            DoFrame(data);
            DoFrame(data);

            var movedBasins = 0;
            foreach (var basin in data.PixMan.Pixels)
            {
                if (basin.HeightOQ > 0) movedBasins++;
            }
            Assert.Less(movedBasins, 13);
        }

        [Test]
        public void HighBasin_62_126_ellipse()
        {
            var data = new BasinDataAbstract<BasinSignedDistance>(new HealpixManager(2));
            data.OnInit();
            data.PixMan.Pixels[45].HeightOQ = 500;
            DoFrame(data);
            
            // Assert.AreEqual(99, data.PixMan.Pixels[33].hOQ, 1);
            Assert.AreEqual(
                99, //// 99 for SignedDistance, 50 for MeanEdge
                data.PixMan.Pixels[62].HeightOQ, 
                1);
            Assert.AreEqual(
                203, //// 203 for SignedDistance, 298 for MeanEdge
                data.PixMan.Pixels[45].HeightOQ, 
                1);

            // Assert.AreEqual(99, data.PixMan.Pixels[77].hOQ, 1);
            DoFrame(data);

            data = new BasinDataAbstract<BasinSignedDistance>(new HealpixManager(2));
            data.PixMan.Pixels[126].HeightOQ = 500;
            DoFrame(data);
            Assert.AreEqual(50, data.PixMan.Pixels[110].HeightOQ, 1);
            Assert.AreEqual(99, data.PixMan.Pixels[142].HeightOQ, 1); //// 99 for SignedDistance, 50 for MeanEdge
        }

        internal static void DoFrame<T>(WaterMoving<T> data) where T : BasinBase
        {
            Console.WriteLine("---------");
            data.DoFrame();
            foreach (var basin in data.PixMan.Pixels)
            {
                if (basin.HeightOQ != 0)
                    Console.WriteLine("{0} {1:#.#}", basin.P, basin.HeightOQ);
            }
        }
    }
}