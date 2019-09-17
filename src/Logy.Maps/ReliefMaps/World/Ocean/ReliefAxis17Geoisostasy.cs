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
                /*Visual = basin =>
                {
                    return basin.GHpureTraverse* 1000;
                }//*/
            };
            var algo = new ShiftAxis(data) { Slow = true, Geoisostasy = true };
            SetData(algo, true);

            Data.Water.Fluidity = fluidity;

            ShiftAxisBalanced(3000);
        }
    }
}