using Logy.Maps.ReliefMaps.Meridian;
using Logy.Maps.ReliefMaps.Meridian.Data;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.Map2D.Tests
{
    [TestFixture]
    public class OptimizationTests : RotationStopMap<MeridianCoorFast>
    {
        [Test]
        public void AnyModificationofGetAltitude()
        {
            Data = new MeridianWater<MeridianCoorFast>(HealpixManager);
            Data.Cycle(1, delegate(int step)
            {
                Data.Draw(Bmp, step);
            });
        }
    }
}