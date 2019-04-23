using Logy.Maps.ReliefMaps.Map2D;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean.Metrics
{
    [TestFixture]
    public class SignedDistanceMap : RotationStopMap<BasinSignedDistance>
    {
        [Test]
        public void Water_ChangeAxis(int i)
        {
            Data = new BasinDataBase<BasinSignedDistance>(HealpixManager, false, false
                //, -200d //, 2000d
            );

            ChangeAxis();
        }
    }
}