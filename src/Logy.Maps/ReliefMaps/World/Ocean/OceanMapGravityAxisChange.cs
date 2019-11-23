using Logy.Maps.Exchange;
using Logy.Maps.Geometry;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    public class OceanMapGravityAxisChange : OceanMap
    {
        public OceanMapGravityAxisChange() : this(6)
        {
        }
        public OceanMapGravityAxisChange(int k) : base(k)
        {
        }

        public string SubdirByDatum(Datum datum)
        {
            return $@"x{datum.X}_y{datum.Y}" + "GravityShift";
        }

        /// <summary>
        /// calculates correction json
        /// </summary>
        [Test]
        public void ChangeAxis_GravityEllipsoidCorrection()
        {
            var newY = 73; // 0, 45, 30, 60
            var newX = -40; // 0
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
                    Gravity = new Gravity { X = newX, Y = newY }
                }
            };

            Subdir = SubdirByDatum(algo.DesiredDatum);

            SetData(algo, true); // inits data

            algo.SetDatum(algo.DesiredDatum, 0);

            Data.DoFrames(
                (frame) =>
                {
                    Draw();
                    SaveBitmap(frame);
                    return 100;
                },
                1000);
        }
    }
}