﻿using Logy.Maps.Exchange;
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
            SetData(new ShiftAxis(data) { Slow = true }, true);

            Data.Water.Fluidity = fluidity; 

            ShiftAxis(
                4000,
                (frame) =>
                {
                    switch (K)
                    {
                        // to make second shift "Y":86.6 on frame 5 and following on frame 61, 121 etc
                        case 7: return frame == 4 ? 4 : 60;
                    }
                    return 10;
                },
                frame =>
                {
                    switch (K)
                    {
                        case 7: return 15 + (frame / 10);
                    }
                    return 15;
                });
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