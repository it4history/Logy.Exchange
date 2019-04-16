using Logy.Maps.ReliefMaps.Map2D;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean.Metrics.Tests
{
    [TestFixture]
    public class SignedDistanceTests : RotationStopMap<BasinSignedDistance>
    {
        protected override int K
        {
            get { return 4; }
        }

        [Test]
        public override void Water_ChangeRotation()
        {
            Data = new BasinDataBase<BasinSignedDistance>(HealpixManager, false, false
                //, -200d //, 2000d
            );

            base.Water_ChangeRotation();
        }
    }
}