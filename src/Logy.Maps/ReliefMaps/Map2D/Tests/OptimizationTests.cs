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
            Data.DoFrames(delegate(int frame)
            {
                Data.Draw(Bmp, frame);
                return 1;
            });
        }
    }
}