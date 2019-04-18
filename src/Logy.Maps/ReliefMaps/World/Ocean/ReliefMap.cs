#if DEBUG
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    [TestFixture]
    public class ReliefMap : OceanMap
    {
        [Test]
        public void RotationStopped()
        {
            Data = new BasinData(HealpixManager, true, false
                , -7000d);
            Data.CheckOcean();

            ChangeRotation(-HealpixManager.Nside, double.MaxValue);
            var framesCountBy2 = 3;
            Data.Cycle(10, delegate(int step) //240 for k8, 150 for k7, 100 for k6
            {
                Data.Draw(Bmp);
                SaveBitmap(step + framesCountBy2);
            }, framesCountBy2);

            Data.RecheckOcean();
        }
    }
}
#endif