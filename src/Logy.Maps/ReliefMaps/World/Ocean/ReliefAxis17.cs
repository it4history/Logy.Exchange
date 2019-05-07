#if DEBUG
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    [TestFixture]
    public class ReliefAxis17 : ReliefMap
    {
        public ReliefAxis17()
        {
            LegendNeeded = true;
        }

        protected override int K
        {
            get { return 6; }
        }

        [Test]
        public void AxisChange_Slow()
        {
            Data = new BasinData(HealpixManager, true)
            {
                IntegrationEndless = true,
            };


            ChangeAxis(
                17,
                1000,
                step =>
                {
                    switch (K)
                    {
                        case 7: return 15 + step / 10;
                    }
                    return 15;
                },
                true,
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

            ChangeAxis();
        }
    }
}
#endif