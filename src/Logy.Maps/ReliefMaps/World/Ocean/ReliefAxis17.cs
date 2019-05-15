#if DEBUG
using Logy.Maps.Exchange;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    /// <summary>
    /// http://hist.tk/hw/Карта_Земли_после_сдвига_полюса_на_17_градусов
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
            SetData(new ShiftAxis(new BasinData(HealpixManager, true)
            {
                IntegrationEndless = true,
            }) { Slow = true });

            ShiftAxis(
                2000,
                frame =>
                {
                    switch (K)
                    {
                        case 7: return 15 + frame / 10;
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
            Data = new BasinData(HealpixManager, true, false
                //, -6000d, null
                //, -1000d, 5000d, true
            )
            {
                IntegrationEndless = true,
            };

            ShiftAxis();
        }
    }
}
#endif