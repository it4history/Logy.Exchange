#if DEBUG
using Logy.Maps.Exchange;
using Logy.Maps.Metrics;
using Logy.Maps.ReliefMaps.Water;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    /// <summary>
    /// http://hist.tk/ory/Карта_Земли_после_сдвига_полюса_на_17_градусов
    /// </summary>
    public class ReliefAxis17 : ReliefMap
    {
        public ReliefAxis17() : this(7)
        {
        }

        public ReliefAxis17(int k) : base(k)
        {
        }

        public ShiftAxis GetAlgorithm()
        {
            return new ShiftAxis(new OceanData(HealpixManager) { WithRelief = true });
        }

        [Test]
        public void AxisChange_Slow()
        {
            var fluidity = WaterModel.FluidityStable;
            var metricType = MetricType.Middle;
            Subdir = fluidity == WaterModel.FluidityStable
                ? null
                : $"fluidity{fluidity} from2761{metricType}";

            var algo = GetAlgorithm();
            algo.Data.MetricType = metricType;
            algo.Slow = true;
            InitData(algo, true);

            Data.Water.Fluidity = fluidity;

            ShiftAxisBalanced(4000);
        }

        /// <summary>
        /// framesCountBy2 == 100 is not enough for all integration
        /// </summary>
        [Test]
        public void AxisChange_Sharp()
        {
            InitDataWithJson(null, GetAlgorithm());

            ShiftAxis(1000);
        }

        [Test]
        public void SmoothingTest()
        {
            InitData(GetAlgorithm(), true);

            Smoothing();

            ShiftAxis(1100);
        }
    }
}
#endif