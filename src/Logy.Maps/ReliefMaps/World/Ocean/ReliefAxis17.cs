﻿#if DEBUG
using Logy.Maps.Exchange;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    /// <summary>
    /// http://hist.tk/ory/Карта_Земли_после_сдвига_полюса_на_17_градусов
    /// </summary>
    [TestFixture]
    public class ReliefAxis17 : ReliefMap
    {
        public ReliefAxis17()
        {
            LegendNeeded = true;
        }

        protected override int K => 7;

        /// <summary>
        /// bug of Logy.Maps.Exchange.Bundle: 
        /// 1. save maps till 500 frame; 
        /// 2. restart from json; 
        /// 3. min at 501 frame is -1221.1, but must be -1221.2
        /// </summary>
        [Test]
        public void AxisChange_Slow()
        {
            var data = new BasinData(HealpixManager)
            {
                WithRelief = true,
                IntegrationEndless = true,
            };
            SetData(new ShiftAxis(data) { Slow = true }, true);

            ShiftAxis(
                2000,
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
        /// http://hist.tk/ory/Рельеф_после_сдвига_полюса_на_17_градусов
        /// </summary>
        [Test]
        public void RelativePaleogeoid()
        {
            SetData(new ShiftAxis { Slow = true }, false, false);

            var algorithm = Bundle.Algorithm as ShiftAxis;
            algorithm.Data.Frame++;
            algorithm.Data.Visual = basin => basin.WaterHeight;
            algorithm.Data.MoveAllWater();
            algorithm.Data.SetScales();
            Draw(algorithm.CurrentPoleBasin, .03);
        }

        /// <summary>
        /// framesCountBy2 == 100 is not enough for all integration
        /// </summary>
        [Test]
        public void AxisChange_Sharp()
        {
            Data = new BasinData(HealpixManager
                /*, -6000d, null
                , -1000d, 5000d, true*/)
            {
                WithRelief = true,
                IntegrationEndless = true,
            };

            ShiftAxis();
        }
    }
}
#endif