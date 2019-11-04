#if DEBUG
using System;
using Logy.Maps.Exchange;
using Logy.Maps.Exchange.Earth2014;
using Logy.Maps.Geometry;
using Logy.Maps.Metrics.Tests;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Map2D;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    public class OceanMap : RotationStopMap<Basin3>
    {
        public OceanMap() : this(5)
        {
        }
        public OceanMap(int k) : base(k)
        {
        }

        [Test]
        public void Water_HighBasin()
        {
            var algorithm = new ShiftAxis(new OceanData(HealpixManager, -20d /*, 200d*/));
            SetData(algorithm);

            var h = 500d;
            var p = HealpixManager.GetP(HealpixManager.Nside + 5, HealpixManager.Nside * 2);
            var basin = Data.PixMan.Pixels[p];
            algorithm.SetDatum(new Datum { PoleBasin = basin });

            basin.Hoq = h;

            Data.PixMan.Pixels[HealpixManager
                .GetP(HealpixManager.Nside, (int)(HealpixManager.Nside * 2.5))].Hoq = h;

            Data.DoFrames(
                delegate(int frame)
                {
                    Draw();
                    SaveBitmap(frame);
                    return 1; /// 240 for k8, 150 for k7, 100 for k6
                },
                20);
        }

        /// <summary>
        /// http://hist.tk/ory/file:OceanMap_Water_HighBasin.gif
        /// </summary>
        [Test]
        public void Water_Gradient()
        {
            var algorithm = new ShiftAxis(new OceanData(HealpixManager, -200d /*, 2000d*/));
            SetData(algorithm);

            var p = HealpixManager.GetP(HealpixManager.Nside - 1, HealpixManager.Nside * 1);
            var basin = Data.PixMan.Pixels[p];
            algorithm.SetDatum(new Datum { PoleBasin = basin });

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
                    Draw();
                    SaveBitmap(frame);
                    return 1;
                },
                30);
        }

        [Test]
        public void Water_ChangeAxis()
        {
            var data = new OceanData(
                HealpixManager
                /*, -2600d, 2700d*/)
            {
                //SamePolesAndEquatorGravitation = true,
                //NoIntegrationFinish = true,
                Visual = (basin, moved) => basin.GVpure * 1000 - 9822
                //basin.r - Earth2014Manager.Radius2Add
            };
            SetData(new ShiftAxis(data)
            {
                // DesiredDatum = new Datum { X = 0, Y = 45 /* 90 */ },
            });
            ShiftAxis(); 
        }

        [Test]
        public void Water_ChangeAxis_Geoisostasy()
        {
            var newX = 30;
            var newY = 45;
            Subdir = $@"x{newX}_y{newY}";
            var algo = new ShiftAxis(new OceanData(HealpixManager)
                {
                    /*
                    Visual = (basin, moved) =>
                    {
                        return basin.Delta_g_traverse * 1000;
                        return basin.Delta_g_meridian * 1000;
                        return basin.RadiusOfEllipse - Earth2014Manager.Radius2Add;
                        return Math.Sqrt(basin.Delta_g_meridian * basin.Delta_g_meridian
                                         + basin.Delta_g_traverse * basin.Delta_g_traverse) * 1000;
                        return basin.GHpure * 1000;
                        return basin.GHpureTraverse * 1000;
                        return (basin.GVpure - EllipsoidAcceleration.GWithAOnEquator) * 1000;
                        return Math.Sqrt(basin.GHpure * basin.GHpure + basin.GHpureTraverse * basin.GHpureTraverse) * 1000;
                    }//*/
                })
            {
                //*
                DesiredDatum = new Datum
                {
                    X = newX,
                    Y = newY,
                   // SiderealDayInSeconds = int.MaxValue,
                   Gravity = new Gravity { X = newX, Y = newY }
                },//*/
                Geoisostasy = true
            };

            SetData(algo); // inits data

            //ShiftAxis(150, null, (frame)=>10);

            //*
            algo.SetDatum(algo.DesiredDatum);//Data.DoFrame();//ShiftAxis(10);

            var p = HealpixManager.GetP(HealpixManager.Nside + 5, HealpixManager.Nside * 2);
            //Data.PixMan.Pixels[p].Hoq = 10000;

            Data.DoFrames(
                (frame) =>
                {
                    Draw();
                    SaveBitmap(frame);
                    return 100;
                },
                1);//*/
            /*Draw();//*/

        }

        [Test]
        public void Hto_Spheric()
        {
            var algorithm = new ShiftAxis(
                new OceanData(new HealpixManager(2)) { Spheric = true });
            SetData(algorithm);

            Data.GradientAndHeightCrosses();
            InitialHtoRecalc(); // may be needed for some metrics

            var basin0 = Data.PixMan.Pixels[0];

            // 44 44 40 40
            // Assert.AreEqual(644, basin0.InitialHto[0] / 10000, 1);
            Assert.AreEqual(0, basin0.RadiusLine.AngleTo(basin0.S_q.Normal).Degrees);

            var basin5 = Data.PixMan.Pixels[5]; // 5,7: 46 42 40 43  
            /// 14,17: 47 41 40 45
            /// 27,31: 47 41 42 42

            algorithm.ChangeRotation(-HealpixManager.Nside);
            Assert.AreEqual(0, basin0.Hto[0]);

            // Data.GradientAndHeightCrosses();
            Assert.AreEqual(0, basin0.Hto[0]);

            OceanDataTests.DoFrame(Data);
        }

        [Test]
        public void Height_RotationStopped()
        {
            var algorithm = new ShiftAxis(
                new OceanData(new HealpixManager(3)) { WithFormattor = false });
            SetData(algorithm);

            algorithm.ChangeRotation(-HealpixManager.Nside);
            
            OceanDataTests.DoFrame(Data, true);
            OceanDataTests.DoFrame(Data, true);
            OceanDataTests.DoFrame(Data, true);
            OceanDataTests.DoFrame(Data, true);
            Draw();
        }

        [Test]
        public void Water_RotationStopped()
        {
            var algorithm = new ShiftAxis(new OceanData(HealpixManager/*,-3000d, 3000d*/));
            SetData(algorithm);
            algorithm.ChangeRotation(-HealpixManager.Nside);
            Data.DoFrames(
                delegate(int frame)
                {
                    Draw();
                    SaveBitmap(frame);
                    return 1;
                },
                30/*400*/);
        }

        private void InitialHtoRecalc()
        {
            foreach (var basin in Data.PixMan.Pixels)
            {
                for (int to = 0; to < 4; to++)
                {
                    basin.HtoBase[to] = 0;
                    basin.HtoBase[to] = basin.Metric(null, to, Data.MetricType);
                }
            }
        }
    }
}
#endif