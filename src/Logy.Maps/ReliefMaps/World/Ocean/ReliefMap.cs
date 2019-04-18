#if DEBUG
using System.Drawing.Imaging;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    [TestFixture]
    public class ReliefMap : OceanMap
    {
        public ReliefMap()
        {
            Scale = 2;
        }

        protected override ImageFormat ImageFormat
        {
            get { return ImageFormat.Bmp; }
        }

        [Test]
        public void RotationStopped()
        {
            Data = new BasinData(HealpixManager, true, false
                //       , -7000d
                , null, null, true
            )
            {
                NoIntegrationFinish = true,
                //Visual = basin => basin.WaterHeight
            };
            Data.CheckOcean();

            ChangeRotation(-HealpixManager.Nside, 5000); // 1000 does nothing
            var framesCountBy2 = 300;
            Data.Cycle(20, delegate(int step) //20 for k7
            {
                Data.Draw(Bmp, 0, null, YResolution, Scale);
                SaveBitmap(step + framesCountBy2);
            }, framesCountBy2);

            Data.RecheckOcean();
        }
    }
}
#endif