#if DEBUG
using Logy.Maps.Exchange;
using Logy.Maps.Geometry;
using Logy.Maps.Metrics;
using Logy.Maps.Projections.Healpix;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    public class OceanMapGravityAxisChange : OceanMap
    {
        private ShiftAxis _algo;

        public OceanMapGravityAxisChange() : this(7)
        {
        }
        public OceanMapGravityAxisChange(int k) : base(k)
        {
        }

        public override void SetUp()
        {
            base.SetUp();
            var datum = Datum.Greenland17; 
            var newY = datum.Y; 
            var newX = datum.X; 
            _algo = new ShiftAxis(new OceanData(HealpixManager)
            {
                MetricType = MetricType.Middle
                /*
                Visual = (basin, moved) =>
                {
                    return basin.Delta_g_traverse * 1000;
                    return basin.Delta_g_meridian * 1000;
                    return basin.RadiusOfGeoid - Earth2014Manager.Radius2Add;
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
                    /// SiderealDayInSeconds = int.MaxValue,
                    Gravity = new Gravity { X = newX, Y = newY }
                }
            };

            Subdir = SubdirByDatum(_algo.DesiredDatum);
        }

        public string SubdirByDatum(Pole datum)
        {
            return $@"x{datum.X}_y{datum.Y}" + "GravityShift";
        }

        /// <summary>
        /// calculates correction json
        /// </summary>
        [Test]
        public void CorrectionCalculated()
        {
            InitDataWithJson(null, _algo); 

            _algo.SetDatum(_algo.DesiredDatum, 0);

            HighFluidity(true);
            Data.DoFrames(
                (frame) => 
                {
                    Draw();
                    SaveBitmap(frame);
                    return 100;
                },
                40);
        }

        [Test]
        public void CorrectionLoadedFromParentResolutionAndCalculated()
        {
            InitData(_algo, true);
            HighFluidity(true);

            var parentMan = new HealpixManager(K - 1);
            var parentBundle = _algo.DesiredDatum.Gravity.LoadCorrection(parentMan.K);
            var parentBasins = parentBundle.Basins[parentMan.K];
            for (var p = 0; p < parentBasins.Length; p++)
                foreach (var kidP in parentMan.GetCenter(p).GetKids(parentMan, HealpixManager))
                    Data.PixMan.Pixels[kidP].Hoq = parentBasins[p].Hoq;

            _algo.SetDatum(_algo.DesiredDatum, 0);

            Data.DoFrames(
                (frame) =>
                {
                    Draw();
                    SaveBitmap(frame);
                    return 100;
                },
                parentBundle.Algorithm.DataAbstract.Frame + 10);
        }
    }
}
#endif