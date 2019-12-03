using System.Drawing.Imaging;
using Logy.Maps.Exchange;
using Logy.Maps.Geometry;
using Logy.Maps.Projections.Healpix;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    public class OceanMapGravityAxisChange : OceanMap
    {
        private ShiftAxis _algo;

        public OceanMapGravityAxisChange() : this(8)
        {
        }
        public OceanMapGravityAxisChange(int k) : base(k)
        {
        }

        protected override ImageFormat ImageFormat => ImageFormat.Tiff;

        public override void SetUp()
        {
            base.SetUp();
            var newY = 73; // 0, 45, 30, 60
            var newX = -40; // 0
            _algo = new ShiftAxis(new OceanData(HealpixManager)
            {
                IntegrationEndless = false,
                /// MetricType = MetricType.MeanEdge
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
                    /// SiderealDayInSeconds = int.MaxValue,
                    Gravity = new Gravity { X = newX, Y = newY }
                }
            };

            Subdir = SubdirByDatum(_algo.DesiredDatum);
        }

        public string SubdirByDatum(Datum datum)
        {
            return $@"x{datum.X}_y{datum.Y}" + "GravityShift";
        }

        /// <summary>
        /// calculates correction json
        /// </summary>
        [Test]
        public void CorrectionCalculated()
        {
            InitData(_algo, true); 

            _algo.SetDatum(_algo.DesiredDatum, 0);

            _algo.Data.IntegrationEndless = false;
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

            var parentMan = new HealpixManager(K - 1);
            var parentBasins = _algo.DesiredDatum.LoadCorrection(parentMan.K).Basins[parentMan.K];
            for (var p = 0; p < parentBasins.Length; p++)
                foreach (var kidP in parentMan.GetCenter(p).GetKids(HealpixManager))
                    Data.PixMan.Pixels[kidP].Hoq = parentBasins[p].Hoq;

            _algo.SetDatum(_algo.DesiredDatum, 0);

            _algo.Data.IntegrationEndless = true;
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