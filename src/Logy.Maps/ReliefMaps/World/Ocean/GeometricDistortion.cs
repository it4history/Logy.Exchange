using System.Drawing.Imaging;
using Logy.Maps.Metrics;
using Logy.Maps.Metrics.Tests;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Map2D;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    /// <summary>
    /// calm water
    /// </summary>
    public class GeometricDistortion : RotationStopMap<Basin3>
    {
        public GeometricDistortion() : base(7)
        {
        }

        protected override ImageFormat ImageFormat => ImageFormat.Png;

        [Test]
        public void EllipticEarth_FirstWave()
        {
            Basin3.MetricType = MetricType.RadiusIntersection;
            var man = new HealpixManager(2);
            var data = new OceanData(man) { Spheric = false };
            data.Init();

            OceanDataTests.DoFrame(data);

            Assert.AreEqual(0, data.PixMan.Pixels[man.Npix / 2].Hoq);

            // distortion on polar cap - equator border
            Assert.AreEqual(36, data.PixMan.Pixels[man.GetP(man.Nside + 1, 1)].Hoq, 1);


            Data = new OceanData(HealpixManager) { ColorsMiddle = .04 };
            Data.Water.Threshhold *= .3;
            OceanDataTests.DoFrame(Data);
            Draw();
            SaveBitmap(0);
        }

        [Test]
        public void Waves100()
        {
            Basin3.MetricType = MetricType.Middle;
            for (int k = 2; k <= 5; k++)
            {
                var man = new HealpixManager(k);
                var data = new OceanData(man) { Spheric = false };
                data.Init();
                for (var i = 0; i < 100; i++)
                    data.DoFrame();

                OceanDataTests.DoFrame(data);

                var movedBasins = 0;
                foreach (var basin in data.PixMan.Pixels)
                {
                    if (basin.Hoq > 0) movedBasins++;
                }
                Assert.Less(movedBasins, 1);
                break;
            }
        }
    }
}