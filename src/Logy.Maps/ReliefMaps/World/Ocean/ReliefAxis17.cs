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
        public ReliefAxis17(int k = 7) : base(k)
        {
        }

        [Test]
        public void AxisChange_Slow()
        {
            var fluidity = WaterModel.FluidityStable;
            var metricType = MetricType.Middle;
            Subdir = fluidity == WaterModel.FluidityStable
                ? null
                : $"fluidity{fluidity} from2761{metricType}";

            var data = new OceanData(HealpixManager)
            {
                WithRelief = true,
                MetricType = metricType,
            };
            InitData(new ShiftAxis(data) { Slow = true }, true);

            Data.Water.Fluidity = fluidity;

            ShiftAxisBalanced(4000);
        }

        /// <summary>
        /// framesCountBy2 == 100 is not enough for all integration
        /// </summary>
        [Test]
        public void AxisChange_Sharp()
        {
            Data = new OceanData(HealpixManager
                /*, -6000d, null
                , -1000d, 5000d, true*/)
            {
                WithRelief = true,
            };

            ShiftAxis();
        }
    }
}