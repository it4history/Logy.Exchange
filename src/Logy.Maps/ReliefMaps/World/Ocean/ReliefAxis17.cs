#if DEBUG
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

        protected override int K => 5;

        [Test]
        public void AxisChange_Slow()
        {
            var data = new BasinData(HealpixManager)
            {
                WithRelief = true,
                IntegrationEndless = true,
            };
            data.Init();
            SetData(new ShiftAxis(data) { Slow = true });

            ShiftAxis(
                2000,
                frame =>
                {
                    switch (K)
                    {
                        case 7: return 15 + (frame / 10);
                    }
                    return 15;
                },
                () =>
                {
                    switch (K)
                    {
                        case 7: return 60;
                    }
                    return 10;
                });
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