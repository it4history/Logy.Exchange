using Logy.Maps.Exchange;
using Logy.Maps.ReliefMaps.Water;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    public class ReliefAxis17Geoisostasy : ReliefMap
    {
        public ReliefAxis17Geoisostasy() : base(7)
        {
        }

        public ReliefAxis17Geoisostasy(int k) : base(k)
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

            SetData(new ShiftAxis(data) { Slow = true, Geoisostasy = true }, true);

            Data.Water.Fluidity = fluidity;

            ShiftAxisBalanced(4000);
        }

    }
}