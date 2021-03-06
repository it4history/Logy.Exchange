﻿#if DEBUG
using System;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Water;
using Logy.Maps.ReliefMaps.World.Ocean;
using NUnit.Framework;

namespace Logy.Maps.Metrics.Tests
{
    [TestFixture]
    public class OceanDataTests
    {
        [Test]
        public void HighBasin_31()
        {
            var data = new OceanData(new HealpixManager(2)) { Spheric = false, WithFormattor = false };
            data.Init();
            var basin3 = data.PixMan.Pixels[31];
            foreach (var basin in data.PixMan.Pixels)
            {
                for (var i = 0; i < 4; i++)
                    Assert.IsFalse(basin.Volumes[i]);
            }

            basin3.Hoq = 500;

            DoFrame(data);
            DoFrame(data);
            DoFrame(data);

            var movedBasins = 0;
            foreach (var basin in data.PixMan.Pixels)
            {
                if (basin.Hoq > 0) movedBasins++;
            }
            Assert.Less(movedBasins, 13);
        }

        [Test]
        public void HighBasin_62_126_ellipse()
        {
            var data = new BasinDataAbstract<SignedDistanceBasin>(new HealpixManager(2));
            data.Init();
            data.PixMan.Pixels[45].Hoq = 500;
            DoFrame(data);
            
            // Assert.AreEqual(99, data.PixMan.Pixels[33].hOQ, 1);
            Assert.AreEqual(
                99, //// 99 for SignedDistance, 50 for MeanEdge
                data.PixMan.Pixels[62].Hoq, 
                1);
            Assert.AreEqual(
                107, //// 203 for SignedDistance, 298 for MeanEdge
                data.PixMan.Pixels[45].Hoq, 
                1);

            // Assert.AreEqual(99, data.PixMan.Pixels[77].hOQ, 1);
            DoFrame(data);

            data = new BasinDataAbstract<SignedDistanceBasin>(new HealpixManager(2));
            data.Init();
            data.PixMan.Pixels[126].Hoq = 500;
            DoFrame(data);
            Assert.AreEqual(99, data.PixMan.Pixels[110].Hoq, 1); /// 99 for SignedDistance
            Assert.AreEqual(99, data.PixMan.Pixels[142].Hoq, 1); /// 99 for SignedDistance, 50 for MeanEdge
        }

        internal static void DoFrame<T>(WaterMoving<T> data, bool mainBasins = false) where T : BasinAbstract
        {
            Console.WriteLine("---------");
            data.DoFrame();
            foreach (var basin in data.PixMan.Pixels)
            {
                if (mainBasins)
                {
                    if (basin.NorthCap == true 
                        && basin.PixelInRing <= Math.Round(basin.PixelsCountInRing / 8d, MidpointRounding.AwayFromZero))
                        Console.WriteLine($"{basin.P}({basin.Ring}) {basin.Hoq:#.#}");
                }
                else if (basin.Hoq != 0)
                    Console.WriteLine($"{basin.P}({basin.Ring}) {basin.Hoq:#.#}");
            }
        }
    }
}
#endif