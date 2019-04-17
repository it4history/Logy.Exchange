#if DEBUG
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Logy.Maps.Projections;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Map2D;
using Logy.Maps.ReliefMaps.World.Ocean.Tests;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    [TestFixture]
    public class OceanMap : RotationStopMap<Basin3>
    {
        protected override int K
        {
            get { return 5; }
        }

        protected override ImageFormat ImageFormat
        {
            get { return ImageFormat.Jpeg; }
        }

        [SetUp]
        public override void SetUp()
        {
            if (Directory.Exists(Dir))
                Directory.Delete(Dir, true);
            base.SetUp();
        }

        [Test]
        public void Water_HighBasin()
        {
            Data = new BasinData(HealpixManager, false, true //true for sphere
                , -20d//, 200d
            );

            var h = 500d;
            var p = HealpixManager.GetP(HealpixManager.Nside + 5, HealpixManager.Nside * 2);
            var basin3 = Data.PixMan.Pixels[p];
            basin3.hOQ = h;
            Data.PixMan.Pixels[HealpixManager
                .GetP(HealpixManager.Nside, (int)(HealpixManager.Nside * 2.5))].hOQ = h;

            var framesCountBy2 = 10;
            Data.Cycle(1, delegate(int step) //240 for k8, 150 for k7, 100 for k6
            {
                Data.Draw(Bmp, 0, null, YResolution, Scale);
                Circle(basin3);
                SaveBitmap(step + framesCountBy2);
            }, framesCountBy2);
            foreach (var basin in Data.PixMan.Pixels)
            {
                if (basin.hOQ<-1000)
                { }
            }
        }

        [Test]
        public void Water_Gradient()
        {
            Data = new BasinData(HealpixManager, false, false //true for sphere
                , -200d //, 2000d
            );

            var p = HealpixManager.GetP(HealpixManager.Nside - 1, HealpixManager.Nside * 1);
            var basin = Data.PixMan.Pixels[p];
            basin.Delta_g_meridian = -.2;

            p = HealpixManager.GetP(HealpixManager.Nside * 2, HealpixManager.Nside * 2);
            Data.PixMan.Pixels[p].Delta_g_meridian = .2;

            p = HealpixManager.GetP(HealpixManager.Nside * 3, HealpixManager.Nside * 2);
            Data.PixMan.Pixels[p].Delta_g_meridian = .2;

            p = HealpixManager.GetP(HealpixManager.Nside + 5, HealpixManager.Nside * 2);
            Data.PixMan.Pixels[p].Delta_g_traverse = .2;


            var framesCountBy2 = 15;
            Data.Cycle(1, delegate(int step)
            {
                Data.Draw(Bmp, 0, null, YResolution, Scale);
                Circle(basin);
                SaveBitmap(step + framesCountBy2);
            }, framesCountBy2);
        }

        [Test]
        public void Hto_Spheric()
        {
            Data = new BasinData(new HealpixManager(2), false, true);

            Data.GradientAndHeightCrosses();
            InitiialHtoRecalc();

            var basin0 = Data.PixMan.Pixels[0];
            // 44 44 40 40
            //Assert.AreEqual(644, basin0.InitialHto[0] / 10000, 1);
            Assert.AreEqual(0, basin0.RadiusLine.Direction.AngleTo(basin0.S_q.Normal).Degrees);

            var basin5 = Data.PixMan.Pixels[5]; // 5,7: 46 42 40 43  
            // 14,17: 47 41 40 45
            // 27,31: 47 41 42 42

            ChangeRotation(-HealpixManager.Nside, double.MaxValue);
            Assert.AreEqual(0, basin0.Hto[0]);

            //Data.GradientAndHeightCrosses();
            Assert.AreEqual(0, basin0.Hto[0]);

            BasinDataTests.Cycle(Data);
        }

        private void InitiialHtoRecalc()
        {
            foreach (var basin in Data.PixMan.Pixels)
            {
                for (int to = 0; to < 4; to++)
                {
                    basin.InitialHto[(int)to] = 0;
                    basin.InitialHto[(int)to] = basin.Metric(null, to);
                }
            }
        }

        [Test]
        public override void Water_ChangeAxis17()
        {
            Data = new BasinData(HealpixManager, false, false
                , -2600d, 2700d
            );

            base.Water_ChangeAxis17();
        }

        [Test]
        public void Water_RotationStopped()
        {
            Data = new BasinData(HealpixManager, false, false
                // ,-3000d, 3000d
            );
            //Data.ColorsMiddle = null;


            ChangeRotation(-HealpixManager.Nside, double.MaxValue);
            var framesCountBy2 = 200;
            Data.Cycle(1, delegate(int step) 
            {
                Data.Draw(Bmp, 0, null, YResolution, Scale);
                SaveBitmap(step + framesCountBy2);
            }, framesCountBy2);
        }

        [Test]
        public void Relief_RotationStopped()
        {
            Data = new BasinData(HealpixManager, true, false
                , -7000d);
            Data.CheckOcean();

            ChangeRotation(-HealpixManager.Nside, double.MaxValue);
            var framesCountBy2 = 3;
            Data.Cycle(10, delegate(int step) //240 for k8, 150 for k7, 100 for k6
            {
                Data.Draw(Bmp);
                SaveBitmap(step + framesCountBy2);
            }, framesCountBy2);

            Data.RecheckOcean();
        }
    }
}
#endif