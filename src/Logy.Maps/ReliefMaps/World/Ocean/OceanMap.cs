﻿#if DEBUG
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Map2D;
using Logy.Maps.ReliefMaps.World.Ocean.Tests;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    [TestFixture]
    public class OceanMap : RotationStopMap<Basin3>
    {
        [Test]
        public void Water_HighBasin()
        {
            Data = new BasinData(HealpixManager, -20d /*, 200d*/)
                { Spheric = true };

            var h = 500d;
            var p = HealpixManager.GetP(HealpixManager.Nside + 5, HealpixManager.Nside * 2);
            var basin3 = Data.PixMan.Pixels[p];
            basin3.HeightOQ = h;
            Data.PixMan.Pixels[HealpixManager
                .GetP(HealpixManager.Nside, (int)(HealpixManager.Nside * 2.5))].HeightOQ = h;

            Data.DoFrames(
                delegate(int frame)
                {
                    Data.Draw(Bmp, 0, null, YResolution, Scale);
                    Circle(basin3);
                    SaveBitmap(frame);
                    return 1; /// 240 for k8, 150 for k7, 100 for k6
                },
                20);
        }

        [Test]
        public void Water_Gradient()
        {
            Data = new BasinData(HealpixManager, -200d /*, 2000d*/);

            var p = HealpixManager.GetP(HealpixManager.Nside - 1, HealpixManager.Nside * 1);
            var basin = Data.PixMan.Pixels[p];
            basin.Delta_g_meridian = -.2;

            p = HealpixManager.GetP(HealpixManager.Nside * 2, HealpixManager.Nside * 2);
            Data.PixMan.Pixels[p].Delta_g_meridian = .2;

            p = HealpixManager.GetP(HealpixManager.Nside * 3, HealpixManager.Nside * 2);
            Data.PixMan.Pixels[p].Delta_g_meridian = .2;

            p = HealpixManager.GetP(HealpixManager.Nside + 5, HealpixManager.Nside * 2);
            Data.PixMan.Pixels[p].Delta_g_traverse = .2;

            Data.DoFrames(
                delegate(int frame)
                {
                    Data.Draw(Bmp, 0, null, YResolution, Scale);
                    Circle(basin);
                    SaveBitmap(frame);
                    return 1;
                },
                30);
        }

        [Test]
        public void Hto_Spheric()
        {
            Data = new BasinData(new HealpixManager(2)) { Spheric = true };

            Data.GradientAndHeightCrosses();
            InitiialHtoRecalc();

            var basin0 = Data.PixMan.Pixels[0];

            // 44 44 40 40
            // Assert.AreEqual(644, basin0.InitialHto[0] / 10000, 1);
            Assert.AreEqual(0, basin0.RadiusLine.Direction.AngleTo(basin0.S_q.Normal).Degrees);

            var basin5 = Data.PixMan.Pixels[5]; // 5,7: 46 42 40 43  
            /// 14,17: 47 41 40 45
            /// 27,31: 47 41 42 42

            ChangeRotation(double.MaxValue, -HealpixManager.Nside);
            Assert.AreEqual(0, basin0.Hto[0]);

            // Data.GradientAndHeightCrosses();
            Assert.AreEqual(0, basin0.Hto[0]);

            BasinDataTests.DoFrame(Data);
        }

        [Test]
        public void Water_ChangeAxis()
        {
            Data = new BasinData(
                HealpixManager
                /*, -2600d, 2700d*/)
            {
                /*SamePolesAndEquatorGravitation = true,
                NoIntegrationFinish = true,
                Visual = basin => basin.r - Earth2014Manager.Radius2Add //*/
            };

            ShiftAxis(); // 45, 90
        }

        [Test]
        public void Water_RotationStopped()
        {
            Data = new BasinData(
                HealpixManager
                /*,-3000d, 3000d*/);

            ChangeRotation(double.MaxValue, -HealpixManager.Nside);
            Data.DoFrames(
                delegate(int frame)
                {
                    Data.Draw(Bmp, 0, null, YResolution, Scale);
                    SaveBitmap(frame);
                    return 1;
                },
                400);
        }

        private void InitiialHtoRecalc()
        {
            foreach (var basin in Data.PixMan.Pixels)
            {
                for (int to = 0; to < 4; to++)
                {
                    basin.InitialHto[to] = 0;
                    basin.InitialHto[to] = basin.Metric(null, to);
                }
            }
        }
    }
}
#endif