using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Map2D;
using NUnit.Framework;

namespace Logy.Maps.Metrics
{
    [TestFixture]
    public class SignedDistanceMap : RotationStopMap<SignedDistanceBasin>
    {
        [Test]
        public void Water_ShiftAxis(int i)
        {
            Data = new BasinDataAbstract<SignedDistanceBasin>(HealpixManager/*, -200d //, 2000d*/);

            ShiftAxis();
        }
    }
}