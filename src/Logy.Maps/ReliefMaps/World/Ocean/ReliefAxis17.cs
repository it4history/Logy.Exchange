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

        [Test]
        public void Sharp_ChangeAxis()
        {
            Data = new BasinData(HealpixManager, true, false
                //, -6000d, null
                //, -1000d, 5000d, true
            )
            {
                NoIntegrationFinish = true,
            };

            ChangeAxis();
        }

        [Test]
        public void Slow_ChangeAxis()
        {
            Data = new BasinData(HealpixManager, true, false
                //, -2000d, 2000d, true
            )
            {
                NoIntegrationFinish = true,
            };


            ChangeAxis(
                17,
                500,
                step =>
                {
                    switch (K)
                    {
                        case 7: return 15 + step / 20;
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
    }
}
#endif