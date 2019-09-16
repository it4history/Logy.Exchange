using Logy.Maps.Exchange;
using Logy.Maps.ReliefMaps.Water;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    public class ReliefAxis17Geoisostasy : ReliefMap
    {
        public ReliefAxis17Geoisostasy() : base(5)
        {
        }

        [Test]
        public void AxisChange()
        {
            var fluidity = WaterModel.FluidityStable;
            Subdir = fluidity == WaterModel.FluidityStable
                ? null
                : $"fluidity{fluidity} from";

            var data = new OceanData(HealpixManager)
            {
                WithRelief = true,
            };
            SetData(new ShiftAxis(data) { Slow = false }, true);

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
    }
}