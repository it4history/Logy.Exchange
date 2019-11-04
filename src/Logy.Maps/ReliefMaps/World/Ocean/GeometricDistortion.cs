#if DEBUG
using System.Drawing.Imaging;
using Logy.Maps.Metrics;
using Logy.Maps.Metrics.Tests;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Map2D;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    /// <summary>
    /// for calm water
    /// </summary>
    public class GeometricDistortion : RotationStopMap<Basin3>
    {
        public GeometricDistortion() : base(5)
        {
        }

        protected override ImageFormat ImageFormat => ImageFormat.Png;

        [Test]
        public void EllipticEarth_FirstWave_RadiusIntersection()
        {
            var man = new HealpixManager(2);
            var data = new OceanData(man)
            {
                Spheric = false,
                MetricType = MetricType.RadiusIntersection
            };
            data.Init();

            OceanDataTests.DoFrame(data);

            Assert.AreEqual(0, data.PixMan.Pixels[man.Npix / 2].Hoq);

            // distortion on polar cap - equator border
            Assert.AreEqual(36, data.PixMan.Pixels[man.GetP(man.Nside + 1, 1)].Hoq, 1);
        }

        [Test]
        public void EllipticEarth_FirstWave()
        {
            LowThreshholdMiddleData();
            OceanDataTests.DoFrame(Data);
            Draw();
            SaveBitmap(0);
        }

        /// <summary>
        /// http://hist.tk/ory/file:OceanMap_GeometricDistortion.png
        /// </summary>
        [Test]
        public void EllipticEarth_5Waves()
        {
            LowThreshholdMiddleData();
            for (var i = 0; i < 5; i++)
                OceanDataTests.DoFrame(Data);

            Draw();
            SaveBitmap(0);
        }

        /*[Test]
        public void EarthShirt17_1Waves()
        {
            Basin3.MetricType = MetricType.Middle;
            var data = new OceanData(HealpixManager) { ColorsMiddle = .04 };
            SetData(new ShiftAxis(data) { Slow = false},false);
            Data.Water.Threshhold *= .05;

            for (var i = 0; i < 1; i++)
                OceanDataTests.DoFrame(Data);

            Draw();
            SaveBitmap(0);
        }*/

        [Test]
        public void Waves100()
        {
            for (int k = 2; k <= 2; k++)
            {
                var man = new HealpixManager(k);
                var data = new OceanData(man)
                {
                    Spheric = false,
                    MetricType = MetricType.Middle
                };
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
            }
        }

        private void LowThreshholdMiddleData()
        {
            Data = new OceanData(HealpixManager)
            {
                ColorsMiddle = .04,
                MetricType = MetricType.Middle
            };
            Data.Water.Threshhold *= .05;
        }
    }
}
#endif