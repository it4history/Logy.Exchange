using Logy.Maps.Exchange;
using Logy.Maps.Geometry;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    public class OceanMapGravityAxisChange : OceanMap
    {
        public OceanMapGravityAxisChange() : base(2)
        {
        }

        [Test]
        public void ChangeAxis_GravityEllipsoidCorrection()
        {
            var newY = 0; // 0, 45, 30, 60
            var newX = 0; // 0
            var withGravity = true;
            Subdir = $@"x{newX}_y{newY}" + (withGravity ? "GravityShift" : null);
            var algo = new ShiftAxis(new OceanData(HealpixManager)
            {
                IntegrationEndless = false,
                // MetricType = MetricType.MeanEdge
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
                DesiredDatum = new Datum
                {
                    X = newX,
                    Y = newY,
                    // SiderealDayInSeconds = int.MaxValue,
                    Gravity = withGravity ? new Gravity { X = newX, Y = newY } : null
                }
            };

            SetData(algo, true); // inits data

            algo.SetDatum(algo.DesiredDatum);

            Data.DoFrames(
                (frame) =>
                {
                    Draw();
                    SaveBitmap(frame);
                    return 1;
                },
                1000);
        }

        [Test]
        public void Water_ChangeAxis_Geoisostasy()
        {
            var newY = 73;
            var newX = -40;
            var algo = new ShiftAxis(new OceanData(HealpixManager)
            {
            })
            {
                DesiredDatum = new Datum
                {
                    X = newX,
                    Y = newY,
                    Gravity = new Gravity { X = newX, Y = newY }
                },
                Geoisostasy = true
            };

            SetData(algo); // inits data

            // ShiftAxis(150, null, (frame)=>10);

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
                1000); //*/
            /*Draw();//*/
        }
    }
}