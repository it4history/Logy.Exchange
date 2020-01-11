using Logy.Maps.Exchange;
using Logy.Maps.Geometry;
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
            var datum = Datum.Greenland17; // Datum.Strahov48;
            var newY = datum.Y; // 45, 30, 60
            var newX = datum.X; // 0
            _algo = new ShiftAxis(new OceanData(HealpixManager)
            {
                /// MetricType = MetricType.MeanEdge
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

            HighFluidity();
            Data.DoFrames(
                (frame) =>
                {
                    Draw();
                    SaveBitmap(frame);
                    return 100;
                },
                1000);
        }

        [Test]
        public void CorrectionLoadedFromParentResolutionAndCalculated()
        {
            InitData(_algo, true);
            HighFluidity();

            var parentMan = new HealpixManager(K - 1);
            var parentBasins = _algo.DesiredDatum.Gravity.LoadCorrection(parentMan.K).Basins[parentMan.K];
            for (var p = 0; p < parentBasins.Length; p++)
                foreach (var kidP in parentMan.GetCenter(p).GetKids(HealpixManager))
                    Data.PixMan.Pixels[kidP].Hoq = parentBasins[p].Hoq;

            _algo.SetDatum(_algo.DesiredDatum, 0);

            Data.DoFrames(
                (frame) =>
                {
                    Draw();
                    SaveBitmap(frame);
                    return 10;
                },
                10);
        }
    }
}