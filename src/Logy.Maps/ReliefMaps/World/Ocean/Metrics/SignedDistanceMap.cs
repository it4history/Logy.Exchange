using Logy.Maps.ReliefMaps.Map2D;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean.Metrics
{
    [TestFixture]
    public class SignedDistanceMap : RotationStopMap<BasinSignedDistance>
    {
        [Test]
        public void Water_ShiftAxis(int i)
        {
            Data = new BasinDataAbstract<BasinSignedDistance>(HealpixManager/*, -200d //, 2000d*/);

            ShiftAxis();
        }
    }
}