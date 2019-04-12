using System;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Map2D;
using Logy.Maps.ReliefMaps.World.Ocean.Tests;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean.Metrics.Tests
{
    [TestFixture]
    public class MeanEdgeMetricTests : RotationStopMap<Basin_MeanEdge>
    {
        protected override int K
        {
            get { return 4; }
        }

        [Test]
        public void Hto_Spheric()
        {
            Data = new BasinDataBase<Basin_MeanEdge>(new HealpixManager(2), false, true);

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

            Data.GradientAndHeightCrosses();
            Assert.AreEqual(0, basin0.Hto[0]);

            BasinDataTests.Cycle(Data);
        }

        [Test]
        public void Water_ChangeRotation()
        {
            Data = new BasinDataBase<Basin_MeanEdge>(HealpixManager, false, false
                //, -200d //, 2000d
            );

            //ChangeRotation(-HealpixManager.Nside, 10000000);
            //Data.GradientAndHeightCrosses();
            //InitiialHtoRecalc();

            var p = HealpixManager.GetP(HealpixManager.Nside - 1, HealpixManager.Nside * 1 - 5);
            var abasin = Data.PixMan.Pixels[p];
            //abasin.Delta_g_meridian = -.2;

            //ChangeRotation(-HealpixManager.Nside, double.MaxValue);

            //Data.PixMan.Pixels[HealpixManager.GetP(HealpixManager.Nside, (int)(HealpixManager.Nside * 2.5))].hOQ = 1000;

            var framesCountBy2 = 5;
            Data.Cycle(2, delegate(int step)
            {
                Data.Draw(Bmp, 0, null, YResolution, Scale);
                SaveBitmap(step + framesCountBy2);
            }, framesCountBy2);
        }

        private void InitiialHtoRecalc()
        {
            foreach (var basin in Data.PixMan.Pixels)
            {
                foreach (Direction to in Enum.GetValues(typeof(Direction)))
                {
                    basin.InitialHto[(int) to] = 0;
                    basin.InitialHto[(int) to] = basin.Metric(null, to);
                }
            }
        }
    }
}